using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

public enum DiscreteMoveType
{
    PromptMoveup,
    PromptMoveDown,
    SetMoveUp,
    SetMoveDown
}

namespace DiscreetTrial
{
    public class Trial
    {
        public string Score = "";
        public string Prompt = "";
        public int Duration;
        public bool Mistrial = false;
    }
    public class Session
    {
        public Trial[] Trials = null;
    }
    public class TrialAvg
    {
        public float AVGScore;
        public float AVGPrompt;
        public float AVGIndependence;
        public float AVGduration;
        public float Correct;
        public float Incorrect;
        public Dictionary<string, float> PromptAverages = new Dictionary<string, float>();

    }
    public class MoveCriteria
    {
        public int BarCondition;
        public int TotalTrial;
        public int SuccessNeeded;
        public bool ConsecutiveSuccess = false;
        public bool bIOAReqd = false;
        public bool bMultiTchr = false;
        public bool ConsecutiveAverage = false; //--- [New Criteria] May 2020 ---//
        public int ConsecutiveAverageValue = 0; //--- [New Criteria] May 2020 ---//

    }
    public class MoveBackCriteria
    {
        public int BarCondition;
        public int TotalTrial;
        public int FailureNeeded;
        public bool ConsecutiveFailures = false;
        public bool bIOAReqd = false;
        public bool bMultiTchr = false;
        public bool ConsecutiveAverageFailure = false; //--- [New Criteria] May 2020 ---//
        public int ConsecutiveAverageFailValue = 0; //--- [New Criteria] May 2020 ---//

    }


    public class InputData
    {
        public bool PromptHirecharchy = false;
        public bool MultiSet = false;
        public bool IOARequired = false;
        public bool MultiTeacherRequired = false;
        public bool IncludeMistrials = false;
        public TrialAvg[] TrialsData = null;
        public string[] PromptsUsed = null;
        public string[] NoPromptsUsed = null;
        public string CurrentPrompt = "";
        public string sCurrentLessonPrompt = "";
        public string TargetPrompt = "";
        public int CurrentSet;
        public int TotalSets;
        public Session[] Sessions = null;
        public int SessionCount = 0;
        public int TrialCount = 0;
        public int promptUp = 0;
        public int promptDown = 0;

        public int MaxSession = 0;

        //Set Move Criteria
        public MoveCriteria PercentAccuracy = new MoveCriteria();
        public MoveCriteria PercentIndependence = new MoveCriteria();
        public MoveCriteria PercentOpportunity = new MoveCriteria();
        public MoveCriteria Rate = new MoveCriteria();


        //Set Move Back Criteria
        public MoveBackCriteria MoveBackPercentAccuracy = new MoveBackCriteria();
        public MoveBackCriteria MoveBackPercentIndependence = new MoveBackCriteria();

        //Prompt Move Criteria
        public MoveCriteria PromptPercentAccuracy = new MoveCriteria();
        public MoveCriteria PromptPercentIndependence = new MoveCriteria();

        //Prompt Move Back Criteria
        public MoveBackCriteria MoveBackPromptPercentAccuracy = new MoveBackCriteria();
        public MoveBackCriteria MoveBackPromptPercentIndependance = new MoveBackCriteria();


        //Custon Column Move Criteria
        public MoveCriteria CustomPercent = new MoveCriteria();
        //public MoveCriteria CustomPercentIndependence = new MoveCriteria();

        //Custon Column Move Back Criteria
        public MoveBackCriteria MoveBackCustom = new MoveBackCriteria();
        //public MoveBackCriteria MoveBackCustomPercentIndependence = new MoveBackCriteria();

        //Duration Column set Move up Criteria
        public MoveCriteria AvgDurationMoveUp = new MoveCriteria();
        public MoveCriteria TotalDurationMoveUp = new MoveCriteria();
        //public MoveCriteria CustomPercentIndependence = new MoveCriteria();

        //Duration Column set Move Back Criteria
        public MoveBackCriteria AvgDurationMoveDown = new MoveBackCriteria();
        public MoveBackCriteria TotalDurationMoveDown = new MoveBackCriteria();
        //public MoveBackCriteria MoveBackCustomPercentIndependence = new MoveBackCriteria();

        //Duration Column prompt Move up Criteria
        public MoveCriteria promptAvgDurationMoveUp = new MoveCriteria();
        public MoveCriteria promptTotalDurationMoveUp = new MoveCriteria();

        //Duration Column prompt Move Back Criteria
        public MoveBackCriteria promptAvgDurationMoveDown = new MoveBackCriteria();
        public MoveBackCriteria promptTotalDurationMoveDown = new MoveBackCriteria();

        //Frequency Column Move Criteria
        public MoveCriteria FrequencyMoveUp = new MoveCriteria();
        public MoveCriteria PromptFrequencyMoveUp = new MoveCriteria();

        //Frequency Column Move Back Criteria
        public MoveBackCriteria FrequencyMoveDown = new MoveBackCriteria();
        public MoveBackCriteria PromptFrequencyMoveDown = new MoveBackCriteria();
        //Total correct for prompt moveup
        public MoveCriteria PromptTotalCorrectMoveUp = new MoveCriteria();

        //Total Incorrect for prompt movedown
        public MoveBackCriteria PromptTotalIncorrectMoveBack = new MoveBackCriteria();
        public MoveBackCriteria PromptTotalCorrectMoveBack = new MoveBackCriteria();

        //Total correct for Set moveup
        public MoveCriteria SetTotalCorrectMoveUp = new MoveCriteria();

        //Total Incorrect for Set movedown
        public MoveBackCriteria SetTotalIncorrectMoveBack = new MoveBackCriteria();
        public MoveBackCriteria SetTotalCorrectMoveBack = new MoveBackCriteria();

        //prompt percent indepedent of all steps
        public MoveCriteria PromptPercentAllIndependence = new MoveCriteria();
        public MoveBackCriteria MoveBackPromptPercentAllIndependance = new MoveBackCriteria();

        //set percent indepedent of all steps
        public MoveCriteria PercentAllIndependence = new MoveCriteria();
        public MoveBackCriteria MoveBackPercentAllIndependence = new MoveBackCriteria();

        //Coded added to check if the selected colum have the criteria for any moveup or movedown.  Arun.
        public bool IsInfluencedBy(DiscreteMoveType moveType)
        {
            bool UseForDecission = false;
            switch (moveType)
            {
                case DiscreteMoveType.PromptMoveup:
                    UseForDecission = (PromptPercentAccuracy.TotalTrial > 0) || (PromptPercentIndependence.TotalTrial > 0) || (PromptFrequencyMoveUp.TotalTrial > 0) || (AvgDurationMoveUp.TotalTrial > 0) || (TotalDurationMoveUp.TotalTrial > 0) || (CustomPercent.TotalTrial > 0) || (PromptTotalCorrectMoveUp.TotalTrial > 0);
                    break;
                case DiscreteMoveType.PromptMoveDown:
                    UseForDecission = (MoveBackPromptPercentAccuracy.TotalTrial > 0) || (MoveBackPromptPercentIndependance.TotalTrial > 0) || (PromptFrequencyMoveDown.TotalTrial > 0) || (AvgDurationMoveDown.TotalTrial > 0) || (TotalDurationMoveDown.TotalTrial > 0) || (MoveBackCustom.TotalTrial > 0) || (PromptTotalIncorrectMoveBack.TotalTrial > 0) || (PromptTotalCorrectMoveBack.TotalTrial > 0);
                    break;
                case DiscreteMoveType.SetMoveUp:
                    UseForDecission = (PercentAccuracy.TotalTrial > 0) || (PercentIndependence.TotalTrial > 0) || (FrequencyMoveUp.TotalTrial > 0) || (AvgDurationMoveUp.TotalTrial > 0) || (TotalDurationMoveUp.TotalTrial > 0) || (CustomPercent.TotalTrial > 0) || (SetTotalCorrectMoveUp.TotalTrial > 0);
                    break;
                case DiscreteMoveType.SetMoveDown:
                    UseForDecission = (MoveBackPercentAccuracy.TotalTrial > 0) || (MoveBackPercentIndependence.TotalTrial > 0) || (FrequencyMoveDown.TotalTrial > 0) || (AvgDurationMoveDown.TotalTrial > 0) || (TotalDurationMoveDown.TotalTrial > 0) || (MoveBackCustom.TotalTrial > 0) || (SetTotalIncorrectMoveBack.TotalTrial > 0) || (SetTotalCorrectMoveBack.TotalTrial > 0);
                    break;
                default:
                    break;

            }

            return UseForDecission;
        }


        static int ToInt(string sValue)
        {
            if (string.IsNullOrEmpty(sValue.Trim()))
            {
                return 0;
            }
            else return int.Parse(sValue);
        }
        public void SetFlags(string sPromptHirecharchy, string sMultiSet, string sIOARequired, string sMultiTeacherRequired, string sIncludeMistrials)
        {
            if (sPromptHirecharchy.Equals("true"))
            {
                PromptHirecharchy = true;
            }
            if (sMultiSet.Equals("true"))
            {
                MultiSet = true;
            }
            if (sIOARequired.Equals("true"))
            {
                IOARequired = true;
            }
            if (sMultiTeacherRequired.Equals("true"))
            {
                MultiTeacherRequired = true;
            }
            if (sIncludeMistrials.Equals("true"))
            {
                IncludeMistrials = true;
            }
        }

        //Arun - Function to get count of Scored Trials.

        public List<int> getScoredTrialCount(ArrayList Trials)
        {
            int Trialcnt = 0;
            List<int> index_list = new List<int>();
            for (int i = 1; i < Trials.Count; i++)
            {
                string[] TrialData = Trials[i].ToString().Split(',');
                if (TrialData[1].ToString().Trim() != "" && TrialData[1].ToString().Trim() != "0" && TrialData[1].ToString().Trim() != "Score")
                {
                    Trialcnt++;
                }
                if (Trials[i].ToString().Contains("Trial"))
                {
                    index_list.Add((Trialcnt));
                    Trialcnt = 0;
                }
                if (i == (Trials.Count - 1))
                {
                    index_list.Add(Trialcnt);
                }

            }
            return index_list;
        }

        public void SetInputData(string sCurrentPrompt, string sTargetPrompt, string sCurrentSet, string sTotalSets, ArrayList Trials)
        {
            CurrentPrompt = sCurrentPrompt;
            TargetPrompt = sTargetPrompt;
            CurrentSet = int.Parse(sCurrentSet);
            TotalSets = int.Parse(sTotalSets);

            TrialsData = new TrialAvg[SessionCount];

            Sessions = new Session[SessionCount];
            int iSessionIndex = -1;
            int iTrialIndex = -1;

            List<int> index_list_trialcount = getScoredTrialCount(Trials);

            List<int> index_list = new List<int>();
            int Trialcnt = 0;
            for (int i = 1; i < Trials.Count; i++)
            {
                Trialcnt++;
                if (Trials[i].ToString().Contains("Trial"))
                {
                    index_list.Add((Trialcnt - 1));
                    Trialcnt = 0;
                }
                if (i == (Trials.Count - 1))
                {
                    index_list.Add(Trialcnt);
                }

            }

            foreach (object oData in Trials)
            {
                string sData = (string)oData;
                if (sData.Contains("Trial"))
                {
                    if (iSessionIndex != -1)
                    {

                        TrialsData[iSessionIndex].AVGScore = TrialsData[iSessionIndex].AVGScore / index_list_trialcount[iSessionIndex] * 100;
                        TrialsData[iSessionIndex].AVGIndependence = TrialsData[iSessionIndex].AVGIndependence / index_list_trialcount[iSessionIndex] * 100;
                        TrialsData[iSessionIndex].AVGduration = TrialsData[iSessionIndex].AVGduration / index_list_trialcount[iSessionIndex];
                        for (int j = 0; j < PromptsUsed.Length; j++)
                        {
                            TrialsData[iSessionIndex].PromptAverages[PromptsUsed[j]] = TrialsData[iSessionIndex].PromptAverages[PromptsUsed[j]] / index_list_trialcount[iSessionIndex];
                        }
                        TrialCount = index_list[(iSessionIndex + 1)];
                    }
                    else
                    {
                        TrialCount = index_list[(iSessionIndex + 1)];
                    }
                    iSessionIndex++;

                    Sessions[iSessionIndex] = new Session();
                    TrialsData[iSessionIndex] = new TrialAvg();
                    for (int j = 0; j < PromptsUsed.Length; j++)
                    {
                        TrialsData[iSessionIndex].PromptAverages.Add(PromptsUsed[j], 0);
                    }

                    Sessions[iSessionIndex].Trials = new Trial[TrialCount];
                    iTrialIndex = 0;
                }
                else
                {
                    string[] sTrial = sData.Split(',');
                    Sessions[iSessionIndex].Trials[iTrialIndex] = new Trial();
                    Sessions[iSessionIndex].Trials[iTrialIndex].Score = sTrial[1].Trim();
                    if (Model.PromptIndex(PromptsUsed, Sessions[iSessionIndex].Trials[iTrialIndex].Score) >= Model.PromptIndex(PromptsUsed, sCurrentPrompt))
                    {
                        TrialsData[iSessionIndex].AVGScore++;
                        TrialsData[iSessionIndex].Correct++;
                    }
                    if (Sessions[iSessionIndex].Trials[iTrialIndex].Score=="-")
                        TrialsData[iSessionIndex].Incorrect++;
                    Sessions[iSessionIndex].Trials[iTrialIndex].Prompt = sTrial[1].Trim();
                    if (Sessions[iSessionIndex].Trials[iTrialIndex].Prompt.Equals(PromptsUsed[PromptsUsed.Length - 1]))
                        TrialsData[iSessionIndex].AVGIndependence++;
                    Sessions[iSessionIndex].Trials[iTrialIndex].Duration = ToInt(sTrial[3]);
                    TrialsData[iSessionIndex].AVGduration += Sessions[iSessionIndex].Trials[iTrialIndex].Duration;

                    for (int j = 0; j < PromptsUsed.Length; j++)
                    {
                        //LIJU if (Sessions[iSessionIndex].Trials[iTrialIndex].Prompt.Equals(PromptsUsed[j]))
                        if (Model.PromptIndex(PromptsUsed, Sessions[iSessionIndex].Trials[iTrialIndex].Score) >= Model.PromptIndex(PromptsUsed, PromptsUsed[j]))
                            TrialsData[iSessionIndex].PromptAverages[PromptsUsed[j]] += 1;
                    }

                    if (sTrial[4].Contains("true"))
                    {
                        Sessions[iSessionIndex].Trials[iTrialIndex].Mistrial = true;
                    }
                    else
                    {
                        Sessions[iSessionIndex].Trials[iTrialIndex].Mistrial = false;
                    }
                    iTrialIndex++;
                }

            }
            TrialsData[iSessionIndex].AVGScore = TrialsData[iSessionIndex].AVGScore / index_list_trialcount[iSessionIndex] * 100;
            TrialsData[iSessionIndex].AVGIndependence = TrialsData[iSessionIndex].AVGIndependence / index_list_trialcount[iSessionIndex] * 100;
            TrialsData[iSessionIndex].AVGduration = TrialsData[iSessionIndex].AVGduration / index_list_trialcount[iSessionIndex];
            for (int j = 0; j < PromptsUsed.Length; j++)
            {
                TrialsData[iSessionIndex].PromptAverages[PromptsUsed[j]] = TrialsData[iSessionIndex].PromptAverages[PromptsUsed[j]] / index_list_trialcount[iSessionIndex];
            }
        }


        public int RequiredSession()
        {
            int max = 0;

            List<int> lTrials = new List<int> { PercentAccuracy.TotalTrial, PercentIndependence.TotalTrial, PercentOpportunity.TotalTrial, 
                MoveBackPercentAccuracy.TotalTrial, MoveBackPercentIndependence.TotalTrial, MoveBackPromptPercentAccuracy.TotalTrial,MoveBackPromptPercentIndependance.TotalTrial,
                PromptPercentAccuracy.TotalTrial, PromptPercentIndependence.TotalTrial,CustomPercent.TotalTrial,MoveBackCustom.TotalTrial,
            AvgDurationMoveUp.TotalTrial,AvgDurationMoveDown.TotalTrial,TotalDurationMoveUp.TotalTrial,TotalDurationMoveDown.TotalTrial,
            FrequencyMoveUp.TotalTrial,FrequencyMoveDown.TotalTrial,SetTotalCorrectMoveUp.TotalTrial,PromptFrequencyMoveUp.TotalTrial,PromptFrequencyMoveDown.TotalTrial,SetTotalIncorrectMoveBack.TotalTrial,SetTotalCorrectMoveBack.TotalTrial,
            PromptTotalCorrectMoveUp.TotalTrial,PercentAllIndependence.TotalTrial,MoveBackPercentAllIndependence.TotalTrial,PromptPercentAllIndependence.TotalTrial,MoveBackPromptPercentAllIndependance.TotalTrial};
            max = lTrials.Max();

            return max;
        }

    }



    //public class Trails
    //{






    //    public ArrayList GetTrialLists(int studentId, int templateID, int nmbrOfSession)
    //    {
    //        ArrayList Trials = new ArrayList();
    //        SqlConnection con = new SqlConnection("Data Source=192.168.1.8;Initial Catalog=AppData;Persist Security Info=True;User ID=dev;Password=test");
    //        string sql = "SELECT S.StdtSessionStepId,S.StepVal, SS.SessionStatusCd, SS.Duration" +
    //                    " FROM StdtSessionDtl S INNER JOIN StdtSessionStep SS ON S.StdtSessionStepId = SS.StdtSessionStepId INNER JOIN StdtSessionHdr SH" +
    //                    " on SS.StdtSessionHdrId=" + templateID + " WHERE S.DSTempSetColId = 5 and SH.CurrentSetId=1";

    //        string line = "";
    //        SqlCommand command = new SqlCommand(sql, con);

    //        con.Open();

    //        SqlDataReader reader = command.ExecuteReader();

    //        line = "Trial,Score,Duration,Mistrial";

    //        Trials.Add(line);

    //        int trailcount = 0;

    //        while (reader.Read())
    //        {
    //            line = reader["StdtSessionStepId"].ToString() + "," + reader["StepVal"].ToString() + "," + reader["SessionStatusCd"].ToString() + ",10,";
    //            //line = reader["StepVal"].ToString() + ",";
    //            //line = reader["SessionStatusCd"].ToString();
    //            Trials.Add(line);
    //            trailcount++;

    //        }

    //        //InputData ip = new InputData();

    //        //ip.TrialCount = trailcount;


    //        reader.Close();

    //        con.Close();


    //        return Trials;
    //    }


    //    public MoveCriteria GetMoveCriteria(int studentId, int templateID, string ruleType, string moveType)
    //    {
    //        MoveCriteria ress = new MoveCriteria();
    //        ress.BarCondition = 80;
    //        ress.SuccessNeeded = 3;
    //        ress.TotalTrial = 5;
    //        ress.ConsecutiveSuccess = false;

    //        return ress;

    //    }

    //    public MoveBackCriteria GetMoveBackCriteria(int studentId, int templateID, string ruleType, string moveType)
    //    {
    //        MoveBackCriteria ress = new MoveBackCriteria();
    //        ress.BarCondition = 80;
    //        ress.FailureNeeded = 3;
    //        ress.TotalTrial = 5;
    //        ress.ConsecutiveFailures = false;

    //        return ress;

    //    }
    //}






    public class Result
    {
        public int NextSet = 0;
        public string NextPrompt = "";
        public string CompletionStatus = "";
        public bool PendingIOA = false;
        public bool PendingMultiTeacher = false;
        public bool MovedBackSet = false;
        public bool MovedBackPrompt = false;
        public bool MovedForwardSet = false;
        public bool MovedForwardPrompt = false;
        public ArrayList Messages = new ArrayList();
    }
    public static class Model
    {
        public static int NoutofnMoveup(float[] sArray, float sCheckFor, int totaltrial)
        {
            int count = 0;
            int startpos = 0;
            if (sArray.Length > totaltrial)
            {
                startpos = sArray.Length - totaltrial;
            }
            else
            {
                startpos = 0;
            }
            for (int i = startpos; i < sArray.Length; i++)
            {
                if (sArray[i] >= sCheckFor)
                {
                    count++;
                }
            }
                return count;
        }
        public static int NoutofnMovedown(float[] sArray, float sCheckFor, int totaltrial)
        {
            int count = 0;
            int startpos = 0;
            if (sArray.Length > totaltrial)
            {
                startpos = sArray.Length - totaltrial;
            }
            else
            {
                startpos = 0;
            }
            for (int i = startpos; i < sArray.Length; i++)
            {
                if (sArray[i] < sCheckFor)
                {
                    count++;
                }
            }
            return count;
        }

        private static int NoutofnMoveupCorrect(float[]  avgData, float sCheckFor, int iTrialCount)
        {
            int maxCount = 0;
            int iStart = avgData.Length - iTrialCount;
            if (iStart < 0)
                iStart = 0;
            for (int i = iStart; i < avgData.Length; i++)
            {
                if (avgData[i] >= sCheckFor)
                {
                    maxCount++;
                }
            }
            return maxCount;
        }
        private static int NoutofnMovedownCorrectorIncorrect(float[] avgData, float sCheckFor, int iTrialCount)
        {
            int maxCount = 0;
            int iStart = avgData.Length - iTrialCount;
            if (iStart < 0)
                iStart = 0;
            for (int i = iStart; i < avgData.Length; i++)
            {
                if (avgData[i] < sCheckFor)
                {
                    maxCount++;
                }
            }
            return maxCount;
        }

        public static int ConsecutiveCount(float[] sArray, float sCheckFor, bool SuccessCount)
        {
            int maxCount = 0;
            int tempCount = 0;
            for (int i = sArray.Length - 1; i >= 0; i--)
            {
                if (!Double.IsNaN(sArray[i]))
                {
                if ((SuccessCount && (sArray[i] < sCheckFor / 100)) || (!SuccessCount && (sArray[i] >= sCheckFor / 100)))
                {
                    if (tempCount > maxCount)
                        maxCount = tempCount;
                    tempCount = 0;
                    break;
                }
                else
                    tempCount++;
            }
            }
            if (tempCount > maxCount)
                maxCount = tempCount;
            return maxCount;
        }

        //--- [New Criteria] May 2020 --|ConsecutiveCountTestNew|--(Start)-- //
        public static int ConsecutiveCountTestNew(float[] sArray, float sCheckFor, bool SuccessCount, int TrialVal)
        {
            int maxCount = 0;
            int tempCount = 0;
            float Consavg = 0;

            for (int i = sArray.Length - 1; i >= 0; i--)
            {
                if (!Double.IsNaN(sArray[i]))
                {
                    Consavg += sArray[i];
                }
            }
            Consavg = Consavg / TrialVal;

            for (int i = sArray.Length - 1; i >= 0; i--)
            {
                if (sArray[i] != -9999)
                {
                    if ((SuccessCount && (Consavg < sCheckFor)) || (!SuccessCount && (Consavg >= sCheckFor)))
                    {
                        if (tempCount > maxCount)
                            maxCount = tempCount;
                        tempCount = 0;
                        break;
                    }
                    else
                        tempCount++;
                }
                else
                    tempCount++;
            }

            if (tempCount > maxCount)
                maxCount = tempCount;

            //new code start
            float Consavgval = 0;
            int iStart = sArray.Length - TrialVal;
            if (iStart < 0)
                iStart = 0;
            for (int i = iStart; i < sArray.Length; i++)
            {
                Consavgval += sArray[i];
        }
            Consavgval = Consavgval / TrialVal;
            
            if (SuccessCount == true)
            {
                if (sArray.Length >= TrialVal && Consavgval >= sCheckFor)
                {
                    return TrialVal;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (sArray.Length >= TrialVal && Consavgval < sCheckFor)
                {
                    return TrialVal;
                }
                else
                {
                    return 0;
                }

            }

            //new code end


           
        }
        //--- [New Criteria] May 2020 --|ConsecutiveCountTestNew|--(End)-- //

        public static int ConsecutiveCountCorrectIncorrect(float[] sArray, float sCheckFor, bool SuccessCount)
        {
            int maxCount = 0;
            int tempCount = 0;
            for (int i = sArray.Length - 1; i >= 0; i--)
            {
                if ((SuccessCount && (sArray[i] < sCheckFor) || (sArray[i] == 0)) || (!SuccessCount && (sArray[i] >= sCheckFor) || (sArray[i] == 0)))
                {
                    if (tempCount > maxCount)
                        maxCount = tempCount;
                    tempCount = 0;
                    break;
                }
                else
                    tempCount++;
            }
            if (tempCount > maxCount)
                maxCount = tempCount;
            return maxCount;
        }

        public static int ConsecutiveCountCorrectIncorrectAvg(float[] sArray, float sCheckFor, bool SuccessCount, int TrialVal)
        {
            float Consavgval = 0;
            int iStart = sArray.Length - TrialVal;
            if (iStart < 0)
                iStart = 0;
            for (int i = iStart; i < sArray.Length; i++)
            {
                Consavgval += sArray[i];
            }
            Consavgval = Consavgval / TrialVal;

            if (SuccessCount)
            {
                if (sArray.Length >= TrialVal && Consavgval >= sCheckFor)
                {
                    return TrialVal;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (sArray.Length >= TrialVal && Consavgval < sCheckFor)
                {
                    return TrialVal;
                }
                else
                {
                    return 0;
                }
            }
               
        }
        private static int ConsecutivePromptAvgCount(TrialAvg[] avgData, float sCheckFor, string currentPrompt)
        {
            int maxCount = 0;
            int tempCount = 0;
            for (int i = avgData.Length - 1; i >= 0; i--)
            {
                if (avgData[i].PromptAverages[currentPrompt] < sCheckFor / 100)
                {
                    if (tempCount > maxCount)
                        maxCount = tempCount;
                    tempCount = 0;
                    break;
                }
                else
                    tempCount++;
            }
            if (tempCount > maxCount)
                maxCount = tempCount;
            return maxCount;
        }

        private static int ConsecutivePromptAvgCountCorrectAvg(TrialAvg[] avgData, float sCheckFor, string currentPrompt, int TrialVal)
        {
            float consavgval = 0;
            int iStart = avgData.Length - TrialVal;
            if (iStart < 0)
                iStart = 0;
            for (int i = iStart; i < avgData.Length; i++)
            {
                consavgval += avgData[i].Correct;
            }
            consavgval = consavgval / TrialVal;
            if (consavgval >= sCheckFor && avgData.Length >= TrialVal)
            {
                return TrialVal;
            }
            else
            {
                return 0;
            }
        }
        private static int ConsecutivePromptAvgCountTestNew(TrialAvg[] avgData, float sCheckFor, string currentPrompt, int maxTrial)
        {
            int maxCount = 0;
            int tempCount = 0;
            float Consavg = 0;
            int iStart = avgData.Length - maxTrial;
            if (iStart < 0)
                iStart = 0;
            for (int i = avgData.Length - 1; i >= iStart; i--)
            {
                if (!Double.IsNaN(avgData[i].PromptAverages[currentPrompt]))
                {
                    Consavg += avgData[i].PromptAverages[currentPrompt];
                }
            }
            Consavg = Consavg / maxTrial;

            for (int i = avgData.Length - 1; i >= iStart; i--)
            {
                if (Consavg < sCheckFor / 100)
                {
                    if (tempCount > maxCount)
                        maxCount = tempCount;
                    tempCount = 0;
                    break;
                }
                else
                    tempCount++;
            }
            if (tempCount > maxCount)
                maxCount = tempCount;
            return maxCount;
        }
        private static int ConsecutivePromptCountCorrect(TrialAvg[] avgData, float sCheckFor, string currentPrompt)
        {
            int maxCount = 0;
            int tempCount = 0;
            for (int i = avgData.Length - 1; i >= 0; i--)
            {
                if (avgData[i].Correct < sCheckFor)
                {
                    if (tempCount > maxCount)
                        maxCount = tempCount;
                    tempCount = 0;
                    break;
                }
                else
                    tempCount++;
            }
            if (tempCount > maxCount)
                maxCount = tempCount;
            return maxCount;
        }
        private static int MoveBackConsecutivePromptAvgCount(TrialAvg[] avgData, float sCheckFor, string currentPrompt)
        {
            int maxCount = 0;
            int tempCount = 0;
            for (int i = avgData.Length - 1; i >= 0; i--)
            {
                if (avgData[i].PromptAverages[currentPrompt] >= sCheckFor / 100)
                {
                    if (tempCount > maxCount)
                        maxCount = tempCount;
                    tempCount = 0;
                    break;
                }
                else
                    tempCount++;
            }
            if (tempCount > maxCount)
                maxCount = tempCount;
            return maxCount;
        }
        private static int MoveBackConsecutivePromptAvgCountNew(TrialAvg[] avgData, float sCheckFor, string currentPrompt,int TrialVal )
        {
            int iStart = avgData.Length - TrialVal;
            if (iStart < 0)
                iStart = 0;
           float Consavgval = 0;
           for (int i = iStart; i < avgData.Length; i++)
            {
                Consavgval += avgData[i].PromptAverages[currentPrompt];
            }
            Consavgval = Consavgval / TrialVal;
            
            // new average check and return
            Consavgval = Consavgval * 100;

            if (avgData.Length >= TrialVal && Consavgval < sCheckFor)
            {
                return TrialVal;
            }
            else
            {
                return 0;
            }

        }


        private static int MoveBackConsecutiveCountCorrectIncorrect(TrialAvg[] avgData, float sCheckFor, string currentPrompt, bool IsTotalCorrect)
        {
            int maxCount = 0;
            int tempCount = 0;
            for (int i = avgData.Length - 1; i >= 0; i--)
            {
                if (IsTotalCorrect)
                {
                    if (avgData[i].Correct >= sCheckFor)
                    {
                        if (tempCount > maxCount)
                            maxCount = tempCount;
                        tempCount = 0;
                        break;
                    }
                    else
                        tempCount++;
                }
                else
                {
                    if (avgData[i].Incorrect >= sCheckFor)
                    {
                        if (tempCount > maxCount)
                            maxCount = tempCount;
                        tempCount = 0;
                        break;
                    }
                    else
                        tempCount++;
                }
            }
            if (tempCount > maxCount)
                maxCount = tempCount;
            return maxCount;
        }

        private static int MoveBackConsecutiveCountCorrectIncorrectAvg(TrialAvg[] avgData, float sCheckFor, string currentPrompt, bool IsTotalCorrect,int TrialVal)
        {
           float Consavgval = 0;
           if (IsTotalCorrect)
           {

               for (int i = 0; i < avgData.Length; i++)
               {
                   Consavgval += avgData[i].Correct;
               }
               Consavgval = Consavgval / TrialVal;
                   if (Consavgval < sCheckFor && avgData.Length >= TrialVal)
                   {
                       return TrialVal;
                   }
                   else
                   {
                       return 0;
                   }
               
           }
           else
           {
               for (int i = 0; i < avgData.Length; i++)
               {
                   Consavgval += avgData[i].Incorrect;
               }
               Consavgval = Consavgval / TrialVal;
                   if (Consavgval < sCheckFor && avgData.Length >= TrialVal)
                   {
                       return TrialVal;
                   }
                   else
                   {
                       return 0;
                   }
               }
           
            
        }

        private static int PromptAvgCount(TrialAvg[] avgData, float sCheckFor, string currentPrompt, int iTrialCount)
        {
            int maxCount = 0;
            int iStart = avgData.Length - iTrialCount;
            if (iStart < 0)
                iStart = 0;
            for (int i = iStart; i < avgData.Length; i++)
            {
                if (avgData[i].PromptAverages[currentPrompt] >= sCheckFor / 100)
                {
                    maxCount++;
                }

            }
            return maxCount;
        }
        private static int PromptCountCorrect(TrialAvg[] avgData, float sCheckFor, string currentPrompt, int iTrialCount)
        {
            int maxCount = 0;
            int iStart = avgData.Length - iTrialCount;
            if (iStart < 0)
                iStart = 0;
            for (int i = iStart; i < avgData.Length; i++)
            {
                if (avgData[i].Correct >= sCheckFor)
                {
                    maxCount++;
                }
            }
            return maxCount;
        }
        private static int MoveBackPromptAvgCount(TrialAvg[] avgData, float sCheckFor, string currentPrompt, int iTrialCount)
        {
            int maxCount = 0;
            int iStart = avgData.Length - iTrialCount;
            if (iStart < 0)
                iStart = 0;
            for (int i = iStart; i < avgData.Length; i++)
            {
                if (avgData[i].PromptAverages[currentPrompt] < sCheckFor / 100)
                {
                    maxCount++;
                }

            }
            return maxCount;
        }
        private static int MoveBackPromptCountCorrectIncorrect(TrialAvg[] avgData, float sCheckFor, string currentPrompt, int iTrialCount, bool IsTotalCorrect)
        {
            int maxCount = 0;
            int iStart = avgData.Length - iTrialCount;
            if (iStart < 0)
                iStart = 0;
            for (int i = iStart; i < avgData.Length; i++)
            {
                if (IsTotalCorrect)
                {
                    if (((avgData[i].Correct < sCheckFor) && (avgData[i].Correct > 0)) || ((avgData[i].Correct == sCheckFor) && (sCheckFor==0)))
                    {
                        maxCount++;
                    }
                }
                else
                {
                    if ((avgData[i].Incorrect <= sCheckFor) && (avgData[i].Incorrect > 0))
                    {
                        maxCount++;
                    }
                }
            }
            return maxCount;
        }
        public static int PromptIndex(string[] promptArray, string sPrompt)
        {

            for (int i = 0; i < promptArray.Length; i++)
            {
                if (sPrompt.Trim().Equals(promptArray[i].Trim()))
                {
                    return i;
                }
            }
            return -1;
        }
        private static bool MoveBackPrompt(string currentPrompt, MoveBackCriteria criteria, TrialAvg[] avgData, string[] promptsUsed)
        {

            int iStartIndex = PromptIndex(promptsUsed, currentPrompt);
            bool bSucceeded = false;
            for (int i = iStartIndex; i < promptsUsed.Length; i++)
            {
                //--- [New Criteria] Jun 2020 --(Start)-- //
                if (criteria.ConsecutiveAverageFailure)
                {
                    int iConsecutiveFailures = MoveBackConsecutivePromptAvgCountNew(avgData, criteria.BarCondition, promptsUsed[i],criteria.TotalTrial);
                    if (iConsecutiveFailures < criteria.FailureNeeded)
                        bSucceeded = true;
                }
                //--- [New Criteria] Jun 2020 --(End)-- //
                else if (criteria.ConsecutiveFailures)
                {

                    int iConsecutiveFailures = MoveBackConsecutivePromptAvgCount(avgData, criteria.BarCondition, promptsUsed[i]);
                    if (iConsecutiveFailures < criteria.FailureNeeded)
                        bSucceeded = true;

                }
                else
                {
                    int iFailures = MoveBackPromptAvgCount(avgData, criteria.BarCondition, promptsUsed[i], criteria.TotalTrial);
                    if (iFailures < criteria.FailureNeeded)
                        bSucceeded = true;
                }
            }

            return !bSucceeded;
        }
        private static bool MoveBackPromptCorrectIncorrect(string currentPrompt, MoveBackCriteria criteria, TrialAvg[] avgData, string[] promptsUsed, bool IsTotalCorrect)
        {

            int iStartIndex = PromptIndex(promptsUsed, currentPrompt);
            bool bSucceeded = false;
            for (int i = iStartIndex; i < promptsUsed.Length; i++)
            {
                //--- [New Criteria] Jun 2020 --(Start)-- //
                if (criteria.ConsecutiveAverageFailure)
                {
                    int iConsecutiveFailures = MoveBackConsecutiveCountCorrectIncorrectAvg(avgData, criteria.BarCondition, promptsUsed[i], IsTotalCorrect, criteria.FailureNeeded);
                    if (iConsecutiveFailures < criteria.FailureNeeded)
                        bSucceeded = true;
                }
                //--- [New Criteria] Jun 2020 --(End)-- //
                else if (criteria.ConsecutiveFailures)
                {

                    int iConsecutiveFailures = MoveBackConsecutiveCountCorrectIncorrect(avgData, criteria.BarCondition, promptsUsed[i], IsTotalCorrect);
                    if (iConsecutiveFailures < criteria.FailureNeeded)
                        bSucceeded = true;

                }
                else
                {
                    int iFailures = MoveBackPromptCountCorrectIncorrect(avgData, criteria.BarCondition, promptsUsed[i], criteria.TotalTrial, IsTotalCorrect);
                    if (iFailures < criteria.FailureNeeded)
                        bSucceeded = true;
                }
            }

            return !bSucceeded;
        }
        private static string GetCurrentPrompt(string currentPrompt, string targetPrompt, MoveCriteria criteria, TrialAvg[] avgData, string[] promptsUsed)
        {
            int iStartIndex = PromptIndex(promptsUsed, currentPrompt);
            int iEndIndex = PromptIndex(promptsUsed, targetPrompt);
            int iLastIndexThatSucceeded = -1;
            for (int i = iStartIndex; i <= iEndIndex; i++)
            {
                //--- [New Criteria] May 2020 --(Start)-- //
                if (criteria.ConsecutiveAverage)
                {
                    int iConsecutiveSuccess = ConsecutivePromptAvgCountTestNew(avgData, criteria.BarCondition, promptsUsed[i],criteria.TotalTrial);
                    if (iConsecutiveSuccess >= criteria.SuccessNeeded)
                        iLastIndexThatSucceeded = i;
                }//--- [New Criteria] May 2020 --(End)-- //
                else if (criteria.ConsecutiveSuccess)
                {

                    int iConsecutiveSuccess = ConsecutivePromptAvgCount(avgData, criteria.BarCondition, promptsUsed[i]);
                    if (iConsecutiveSuccess >= criteria.SuccessNeeded)
                        iLastIndexThatSucceeded = i;

                }
                else
                {
                    int iSuccess = PromptAvgCount(avgData, criteria.BarCondition, promptsUsed[i], criteria.TotalTrial);

                    if (iSuccess >= criteria.SuccessNeeded)
                        iLastIndexThatSucceeded = i;
                }

            }
            if (iLastIndexThatSucceeded == -1)
                return currentPrompt;
            if (iLastIndexThatSucceeded == promptsUsed.Length - 1)
                return "*";
            else
            {
                return promptsUsed[iLastIndexThatSucceeded + 1];
            }
        }
        private static string GetCurrentPromptCorrect(string currentPrompt, string targetPrompt, MoveCriteria criteria, TrialAvg[] avgData, string[] promptsUsed)
        {
            int iStartIndex = PromptIndex(promptsUsed, currentPrompt);
            int iEndIndex = PromptIndex(promptsUsed, targetPrompt);
            int iLastIndexThatSucceeded = -1;
            for (int i = iStartIndex; i <= iEndIndex; i++)
            {

                //--- [New Criteria] May 2020 --(Start)-- //
                if (criteria.ConsecutiveAverage)
                {
                    int iConsecutiveSuccess = ConsecutivePromptAvgCountCorrectAvg(avgData, criteria.BarCondition, promptsUsed[i],criteria.TotalTrial);
                    if (iConsecutiveSuccess >= criteria.SuccessNeeded)
                        iLastIndexThatSucceeded = i;
                }//--- [New Criteria] May 2020 --(End)-- //
                else if (criteria.ConsecutiveSuccess)
                {

                    int iConsecutiveSuccess = ConsecutivePromptCountCorrect(avgData, criteria.BarCondition, promptsUsed[i]);
                    if (iConsecutiveSuccess >= criteria.SuccessNeeded)
                        iLastIndexThatSucceeded = i;

                }
                else
                {
                    int iSuccess = PromptCountCorrect(avgData, criteria.BarCondition, promptsUsed[i], criteria.TotalTrial);

                    if (iSuccess >= criteria.SuccessNeeded)
                        iLastIndexThatSucceeded = i;
                }

            }
            if (iLastIndexThatSucceeded == -1)
                return currentPrompt;
            if (iLastIndexThatSucceeded == promptsUsed.Length - 1)
                return "*";
            else
            {
                return promptsUsed[iLastIndexThatSucceeded + 1];
            }
        }
        public static Result Execute(InputData Data, bool bpromptColumn)
        {
            Result res = new Result();


            float[] AVGScoreArray = new float[Data.TrialsData.Length];
            float[] AVGIndependenceArray = new float[Data.TrialsData.Length];
            float[] AVGPromptArray = new float[Data.TrialsData.Length];
            float[] AVGCorrect = new float[Data.TrialsData.Length];
            float[] AVGIncorrect = new float[Data.TrialsData.Length];
            int iAccuracyCount = 0;
            int iIndCount = 0;
            int iFailedAccuracyCount = 0;
            int iFailedIndCount = 0;
            int iCorrectCount = 0;
            int iInCorrectCount = 0;
            int iFailedCorrectCount = 0;
            int iConsecutiveAccuracyCount = 0;
            int iConsecutiveIndCount = 0;
            int iConsecutiveFailureAccuracyCount = 0;
            int iConsecutiveFailureIndCount = 0;
            int iConsecutiveCorrectCount = 0;
            int iConsecutiveFailureCorrectCount = 0;
            int iConsecutiveFailureInCorrectCount = 0;

            int iConsecutiveAvgAccuracyCount = 0; //--- [New Criteria] May 2020 --- //
            int iConsecutiveAvgIndCount = 0; //--- [New Criteria] May 2020 --- //
            int iConsecutiveAvgFailureAccuracyCount = 0; //--- [New Criteria] May 2020 --- //
            int iConsecutiveAvgFailureIndCount = 0; //--- [New Criteria] May 2020 --- //
            int iConsecutiveAvgCorrectCount = 0; //--- [New Criteria] May 2020 --- //
            int iConsecutiveAvgFailureCorrectCount = 0; //--- [New Criteria] May 2020 --- //
            int iConsecutiveAvgFailureInCorrectCount = 0; //--- [New Criteria] May 2020 --- //


            for (int i = 0; i < Data.TrialsData.Length; i++)
            {
                AVGScoreArray[i] = Data.TrialsData[i].AVGScore;
                AVGIndependenceArray[i] = Data.TrialsData[i].AVGIndependence;
                AVGCorrect[i] = Data.TrialsData[i].Correct;
                AVGIncorrect[i] = Data.TrialsData[i].Incorrect;

                //if (Data.TrialsData[i].AVGScore >= Data.PercentAccuracy.BarCondition)
                //{
                //    iAccuracyCount++;
                //}
                //if (Data.TrialsData[i].AVGIndependence >= Data.PercentIndependence.BarCondition)
                //{
                //    iIndCount++;
                //}
                //if (Data.TrialsData[i].AVGScore < Data.MoveBackPercentAccuracy.BarCondition)
                //{
                //    iFailedAccuracyCount++;
                //}
                ////if (Data.TrialsData[i].AVGIndependence < Data.MoveBackPercentIndependence.BarCondition)
                ////{
                ////    iFailedIndCount++;
                ////}
                //if(Data.TrialsData[i].Correct >= Data.SetTotalCorrectMoveUp.BarCondition)
                //{
                //    iCorrectCount++;
                //}
                //if (Data.TrialsData[i].Correct < Data.SetTotalCorrectMoveBack.BarCondition && Data.TrialsData[i].Correct > 0)
                //{
                //    iFailedCorrectCount++;
                //}
                //if (Data.TrialsData[i].Incorrect < Data.SetTotalIncorrectMoveBack.BarCondition && Data.TrialsData[i].Incorrect > 0)
                //{
                //    iInCorrectCount++;
                //}
                }
            iAccuracyCount = NoutofnMoveup(AVGScoreArray, Data.PercentAccuracy.BarCondition, Data.PercentAccuracy.TotalTrial);
             iFailedAccuracyCount = NoutofnMovedown(AVGScoreArray, Data.MoveBackPercentAccuracy.BarCondition, Data.MoveBackPercentAccuracy.TotalTrial);
             iIndCount = NoutofnMoveup(AVGIndependenceArray, Data.PercentIndependence.BarCondition, Data.PercentIndependence.TotalTrial);
             iFailedIndCount = NoutofnMovedown(AVGIndependenceArray, Data.MoveBackPercentIndependence.BarCondition, Data.MoveBackPercentIndependence.TotalTrial);
             iCorrectCount = NoutofnMoveupCorrect(AVGCorrect, Data.SetTotalCorrectMoveUp.BarCondition, Data.SetTotalCorrectMoveUp.TotalTrial);
             iFailedCorrectCount = NoutofnMovedownCorrectorIncorrect(AVGCorrect, Data.SetTotalCorrectMoveBack.BarCondition, Data.SetTotalCorrectMoveBack.TotalTrial);
             iInCorrectCount = NoutofnMovedownCorrectorIncorrect(AVGIncorrect, Data.SetTotalIncorrectMoveBack.BarCondition, Data.SetTotalIncorrectMoveBack.TotalTrial);

            iConsecutiveAccuracyCount = ConsecutiveCount(AVGScoreArray, Data.PercentAccuracy.BarCondition, true);
            iConsecutiveIndCount = ConsecutiveCount(AVGIndependenceArray, Data.PercentIndependence.BarCondition, true);
            iConsecutiveFailureAccuracyCount = ConsecutiveCount(AVGScoreArray, Data.MoveBackPercentAccuracy.BarCondition, false);
            iConsecutiveFailureIndCount = ConsecutiveCount(AVGIndependenceArray, Data.MoveBackPercentIndependence.BarCondition, false);
            iConsecutiveCorrectCount = ConsecutiveCountCorrectIncorrect(AVGCorrect, Data.SetTotalCorrectMoveUp.BarCondition, true);
            iConsecutiveFailureCorrectCount = ConsecutiveCountCorrectIncorrect(AVGCorrect, Data.SetTotalCorrectMoveBack.BarCondition, false);
            iConsecutiveFailureInCorrectCount = ConsecutiveCountCorrectIncorrect(AVGIncorrect, Data.SetTotalIncorrectMoveBack.BarCondition, false);

            iConsecutiveAvgAccuracyCount = ConsecutiveCountTestNew(AVGScoreArray, Data.PercentAccuracy.BarCondition, true, Data.PercentAccuracy.SuccessNeeded); //--- [New Criteria] May 2020 --- //
            iConsecutiveAvgIndCount = ConsecutiveCountTestNew(AVGIndependenceArray, Data.PercentIndependence.BarCondition, true, Data.PercentIndependence.SuccessNeeded); //--- [New Criteria] May 2020 --- //
            iConsecutiveAvgFailureAccuracyCount = ConsecutiveCountTestNew(AVGScoreArray, Data.MoveBackPercentAccuracy.BarCondition, false, Data.MoveBackPercentAccuracy.FailureNeeded); //--- [New Criteria] May 2020 --- //
            iConsecutiveAvgFailureIndCount = ConsecutiveCountTestNew(AVGIndependenceArray, Data.MoveBackPercentIndependence.BarCondition, false, Data.MoveBackPercentIndependence.FailureNeeded); //--- [New Criteria] May 2020 --- //
            iConsecutiveAvgCorrectCount = ConsecutiveCountCorrectIncorrectAvg(AVGCorrect, Data.SetTotalCorrectMoveUp.BarCondition, true, Data.SetTotalCorrectMoveUp.SuccessNeeded); //--- [New Criteria] May 2020 --- //
            iConsecutiveAvgFailureCorrectCount = ConsecutiveCountCorrectIncorrectAvg(AVGCorrect, Data.SetTotalCorrectMoveBack.BarCondition, false, Data.SetTotalCorrectMoveBack.FailureNeeded); //--- [New Criteria] May 2020 --- //
            iConsecutiveAvgFailureInCorrectCount = ConsecutiveCountCorrectIncorrectAvg(AVGIncorrect, Data.SetTotalIncorrectMoveBack.BarCondition, false, Data.SetTotalIncorrectMoveBack.FailureNeeded); //--- [New Criteria] May 2020 --- //
             

            res.CompletionStatus = "NOT COMPLETED";
            res.NextSet = Data.CurrentSet;
            res.NextPrompt = Data.CurrentPrompt;



            //Check Prompt Move Criteria if its Prompt Hirearchy
            string sNewPrompt = "";
            if (Data.PromptHirecharchy)
            {
                if (Data.PromptPercentAccuracy.TotalTrial > 0)
                {
                    sNewPrompt = GetCurrentPrompt(Data.CurrentPrompt, Data.TargetPrompt, Data.PromptPercentAccuracy, Data.TrialsData, Data.PromptsUsed);
                }
                if (Data.PromptPercentIndependence.TotalTrial > 0)
                {
                    sNewPrompt = GetCurrentPrompt(Data.CurrentPrompt, Data.TargetPrompt, Data.PromptPercentIndependence, Data.TrialsData, Data.PromptsUsed);
                }
                if (Data.PromptTotalCorrectMoveUp.TotalTrial > 0)
                {
                    sNewPrompt = GetCurrentPromptCorrect(Data.CurrentPrompt, Data.TargetPrompt, Data.PromptTotalCorrectMoveUp, Data.TrialsData, Data.PromptsUsed);
                }
                if (sNewPrompt.Equals("*") || PromptIndex(Data.PromptsUsed, sNewPrompt) > PromptIndex(Data.PromptsUsed, Data.CurrentPrompt))
                {
                    res.MovedForwardPrompt = true;
                }
                else
                {
                    res.MovedForwardPrompt = false;
                }
                if (!sNewPrompt.Equals("*"))
                    res.NextPrompt = sNewPrompt;
                else
                    res.NextPrompt = Data.PromptsUsed[Data.PromptsUsed.Length - 1];
                //Should we check for criteria of Independence???
            }

            //Check for Move Back Criteria for Prompt if its Prompt Hirearchy
            if (!res.MovedForwardPrompt)
            {
                if (Data.PromptHirecharchy)
                {

                    if (Data.MoveBackPromptPercentAccuracy.TotalTrial > 0)
                    {

                        res.MovedBackPrompt = MoveBackPrompt(Data.CurrentPrompt, Data.MoveBackPromptPercentAccuracy, Data.TrialsData, Data.PromptsUsed);
                    }

                    if (Data.MoveBackPromptPercentIndependance.TotalTrial > 0)
                    {
                        res.MovedBackPrompt = MoveBackPrompt(Data.CurrentPrompt, Data.MoveBackPromptPercentIndependance, Data.TrialsData, Data.PromptsUsed);
                    }
                    if (Data.PromptTotalCorrectMoveBack.TotalTrial > 0)
                    {

                        res.MovedBackPrompt = MoveBackPromptCorrectIncorrect(Data.CurrentPrompt, Data.PromptTotalCorrectMoveBack, Data.TrialsData, Data.PromptsUsed,true);
                    }
                    if (Data.PromptTotalIncorrectMoveBack.TotalTrial > 0)
                    {

                        res.MovedBackPrompt = MoveBackPromptCorrectIncorrect(Data.CurrentPrompt, Data.PromptTotalIncorrectMoveBack, Data.TrialsData, Data.PromptsUsed,false);
                    }
                    if (res.MovedBackPrompt)
                    {
                        int iPromptIndex = 0;

                        if (Data.CurrentPrompt == "+")
                        {
                            iPromptIndex = PromptIndex(Data.NoPromptsUsed, Data.sCurrentLessonPrompt);
                            if (iPromptIndex > 0)
                                res.NextPrompt = Data.NoPromptsUsed[iPromptIndex - 1];
                            else
                                res.MovedBackPrompt = false;
                        }
                        else
                        {
                            iPromptIndex = PromptIndex(Data.PromptsUsed, Data.CurrentPrompt);
                            if (iPromptIndex > 0)
                                res.NextPrompt = Data.PromptsUsed[iPromptIndex - 1];
                            else
                                res.MovedBackPrompt = false;
                        }

                        //iPromptIndex = PromptIndex(Data.PromptsUsed, Data.CurrentPrompt);
                        //if (iPromptIndex > 0)
                        //    res.NextPrompt = Data.PromptsUsed[iPromptIndex - 1];
                        //else
                        //    res.MovedBackPrompt = false;
                    }
                }
            }
            bool bSetMove = false;
            //If NOT Prompt Hirearchy or Current Prompt == "*"  then check for SET move criteria
            if (Data.PercentAccuracy.TotalTrial > 0 || Data.PercentIndependence.TotalTrial > 0 || Data.SetTotalCorrectMoveUp.TotalTrial > 0)
            {
                bSetMove = true;
                //if (!Data.PromptHirecharchy || sNewPrompt == "*") old code.
                if (!bpromptColumn)
                {
                    if (Data.NoPromptsUsed.Length > 0)
                    {
                        if ((Data.sCurrentLessonPrompt == Data.NoPromptsUsed[Data.NoPromptsUsed.Length - 1]) || (Data.promptDown >= 1)) // or prompt criteria is NA
                        {
                            if (Data.PercentAccuracy.TotalTrial > 0)
                            {
                                if ((Data.PercentAccuracy.SuccessNeeded > iAccuracyCount) && !Data.PercentAccuracy.ConsecutiveAverage)
                                {
                                    bSetMove = false;
                                }//--- [New Criteria] May 2020 --|PercentAccuracy|--(Start)-- //
                                else if (Data.PercentAccuracy.ConsecutiveAverage && (Data.PercentAccuracy.SuccessNeeded > iConsecutiveAvgAccuracyCount))
                                {
                                    bSetMove = false;
                                }//--- [New Criteria] May 2020 --|PercentAccuracy|--(End)-- //
                                else if (Data.PercentAccuracy.ConsecutiveSuccess && (Data.PercentAccuracy.SuccessNeeded > iConsecutiveAccuracyCount))
                                {
                                    bSetMove = false;
                                }
                                else if ((Data.sCurrentLessonPrompt != Data.NoPromptsUsed[Data.NoPromptsUsed.Length - 1]) && ((Data.PercentAccuracy.SuccessNeeded == iAccuracyCount) || (Data.PercentAccuracy.SuccessNeeded > iConsecutiveAvgAccuracyCount) || (Data.PercentAccuracy.SuccessNeeded > iConsecutiveAccuracyCount)))
                                {
                                    bSetMove = false;
                                }
                            }
                            if (Data.PercentIndependence.TotalTrial > 0)
                            {
                                if (Data.PercentIndependence.SuccessNeeded > iIndCount && !Data.PercentIndependence.ConsecutiveAverage)
                                {
                                    bSetMove = false;
                                }//--- [New Criteria] May 2020 --|PercentIndependence|--(Start)-- //
                                else if (Data.PercentIndependence.ConsecutiveAverage && (Data.PercentIndependence.SuccessNeeded > iConsecutiveAvgIndCount))
                                {
                                    bSetMove = false;
                                }//--- [New Criteria] May 2020 --|PercentIndependence|--(End)-- //
                                else if (Data.PercentIndependence.ConsecutiveSuccess && (Data.PercentIndependence.SuccessNeeded > iConsecutiveIndCount))
                                {
                                    bSetMove = false;
                                }
                                else if ((Data.sCurrentLessonPrompt != Data.NoPromptsUsed[Data.NoPromptsUsed.Length - 1]) && ((Data.PercentAccuracy.SuccessNeeded == iIndCount) || (Data.PercentAccuracy.SuccessNeeded > iConsecutiveAvgIndCount) || (Data.PercentAccuracy.SuccessNeeded > iConsecutiveIndCount)))
                                {
                                    bSetMove = false;
                                }
                            }
                            if (Data.SetTotalCorrectMoveUp.TotalTrial > 0)
                            {
                                if (Data.SetTotalCorrectMoveUp.SuccessNeeded > iCorrectCount && !Data.SetTotalCorrectMoveUp.ConsecutiveAverage)
                                {
                                    bSetMove = false;
                                }//--- [New Criteria] May 2020 --|SetTotalCorrectMoveUp|--(Start)-- //
                                else if (Data.SetTotalCorrectMoveUp.ConsecutiveAverage && (Data.SetTotalCorrectMoveUp.SuccessNeeded > iConsecutiveAvgCorrectCount))
                                {
                                    bSetMove = false;
                                }//--- [New Criteria] May 2020 --|SetTotalCorrectMoveUp|--(End)-- //
                                else if (Data.SetTotalCorrectMoveUp.ConsecutiveSuccess && (Data.SetTotalCorrectMoveUp.SuccessNeeded > iConsecutiveCorrectCount))
                                {
                                    bSetMove = false;
                                }
                                else if ((Data.sCurrentLessonPrompt != Data.NoPromptsUsed[Data.NoPromptsUsed.Length - 1]) && ((Data.PercentAccuracy.SuccessNeeded == iCorrectCount) || (Data.PercentAccuracy.SuccessNeeded > iConsecutiveAvgCorrectCount) || (Data.PercentAccuracy.SuccessNeeded > iConsecutiveCorrectCount)))
                                {
                                    bSetMove = false;
                                }
                            }
                        }

                        else
                        {
                            bSetMove = false;
                        }
                    }
                    else
                    {
                        if (Data.CurrentPrompt == Data.TargetPrompt)
                        {
                            if (Data.PercentAccuracy.TotalTrial > 0)
                            {
                                if (Data.PercentAccuracy.SuccessNeeded > iAccuracyCount && !Data.PercentAccuracy.ConsecutiveAverage)
                                {
                                    bSetMove = false;
                                }//--- [New Criteria] May 2020 --|PercentAccuracy|--(Start)-- //
                                else if (Data.PercentAccuracy.ConsecutiveAverage && (Data.PercentAccuracy.SuccessNeeded > iConsecutiveAvgAccuracyCount))
                                {
                                    bSetMove = false;
                                }//--- [New Criteria] May 2020 --|PercentAccuracy|--(End)-- //
                                else if (Data.PercentAccuracy.ConsecutiveSuccess && (Data.PercentAccuracy.SuccessNeeded > iConsecutiveAccuracyCount))
                                {
                                    bSetMove = false;
                                }
                            }
                            if (Data.PercentIndependence.TotalTrial > 0)
                            {
                                if (Data.PercentIndependence.SuccessNeeded > iIndCount && !Data.PercentIndependence.ConsecutiveAverage)
                                {
                                    bSetMove = false;
                                }//--- [New Criteria] May 2020 --|PercentIndependence|--(Start)-- //
                                else if (Data.PercentIndependence.ConsecutiveAverage && (Data.PercentIndependence.SuccessNeeded > iConsecutiveAvgIndCount))
                                {
                                    bSetMove = false;
                                }//--- [New Criteria] May 2020 --|PercentIndependence|--(End)-- //
                                else if (Data.PercentIndependence.ConsecutiveSuccess && (Data.PercentIndependence.SuccessNeeded > iConsecutiveIndCount))
                                {
                                    bSetMove = false;
                                }
                            }
                            if (Data.SetTotalCorrectMoveUp.TotalTrial > 0)
                            {
                                if (Data.SetTotalCorrectMoveUp.SuccessNeeded > iCorrectCount && !Data.SetTotalCorrectMoveUp.ConsecutiveAverage)
                                {
                                    bSetMove = false;
                                }//--- [New Criteria] May 2020 --|SetTotalCorrectMoveUp|--(Start)-- //
                                else if (Data.SetTotalCorrectMoveUp.ConsecutiveAverage && (Data.SetTotalCorrectMoveUp.SuccessNeeded > iConsecutiveAvgCorrectCount))
                                {
                                    bSetMove = false;
                                }//--- [New Criteria] May 2020 --|SetTotalCorrectMoveUp|--(End)-- //
                                else if (Data.SetTotalCorrectMoveUp.ConsecutiveSuccess && (Data.SetTotalCorrectMoveUp.SuccessNeeded > iConsecutiveCorrectCount))
                                {
                                    bSetMove = false;
                                }
                            }
                        }
                        else
                        {
                            bSetMove = false;
                        }
                    }

                }
                else
                {
                    if (Data.CurrentPrompt == Data.TargetPrompt)
                    {
                        if (Data.PercentAccuracy.TotalTrial > 0)
                        {
                            if (Data.PercentAccuracy.SuccessNeeded > iAccuracyCount && !Data.PercentAccuracy.ConsecutiveAverage)
                            {
                                bSetMove = false;
                            }//--- [New Criteria] May 2020 --|PercentAccuracy|--(Start)-- //
                            else if (Data.PercentAccuracy.ConsecutiveAverage && (Data.PercentAccuracy.SuccessNeeded > iConsecutiveAvgAccuracyCount))
                            {
                                bSetMove = false;
                            }//--- [New Criteria] May 2020 --|PercentAccuracy|--(End)-- //
                            else if (Data.PercentAccuracy.ConsecutiveSuccess && (Data.PercentAccuracy.SuccessNeeded > iConsecutiveAccuracyCount))
                            {
                                bSetMove = false;
                            }
                        }
                        if (Data.PercentIndependence.TotalTrial > 0)
                        {
                            if (Data.PercentIndependence.SuccessNeeded > iIndCount && !Data.PercentIndependence.ConsecutiveAverage)
                            {
                                bSetMove = false;
                            }//--- [New Criteria] May 2020 --|PercentIndependence|--(Start)-- //
                            else if (Data.PercentIndependence.ConsecutiveAverage && (Data.PercentIndependence.SuccessNeeded > iConsecutiveAvgIndCount))
                            {
                                bSetMove = false;
                            }//--- [New Criteria] May 2020 --|PercentIndependence|--(End)-- //
                            else if (Data.PercentIndependence.ConsecutiveSuccess && (Data.PercentIndependence.SuccessNeeded > iConsecutiveIndCount))
                            {
                                bSetMove = false;
                            }
                        }
                        if (Data.SetTotalCorrectMoveUp.TotalTrial > 0)
                        {
                            if (Data.SetTotalCorrectMoveUp.SuccessNeeded > iCorrectCount && !Data.SetTotalCorrectMoveUp.ConsecutiveAverage)
                            {
                                bSetMove = false;
                            }//--- [New Criteria] May 2020 --|SetTotalCorrectMoveUp|--(Start)-- //
                            else if (Data.SetTotalCorrectMoveUp.ConsecutiveAverage && (Data.SetTotalCorrectMoveUp.SuccessNeeded > iConsecutiveAvgCorrectCount))
                            {
                                bSetMove = false;
                            }//--- [New Criteria] May 2020 --|SetTotalCorrectMoveUp|--(End)-- //
                            else if (Data.SetTotalCorrectMoveUp.ConsecutiveSuccess && (Data.SetTotalCorrectMoveUp.SuccessNeeded > iConsecutiveCorrectCount))
                            {
                                bSetMove = false;
                            }
                        }
                    }
                    else
                    {
                        bSetMove = false;
                    }
                }
            }
            res.MovedForwardSet = bSetMove;



            if (!res.MovedForwardSet)
            {
                //Check for Move back Criteria for Set.
                if (bpromptColumn)
                {
                    if (Data.MoveBackPercentAccuracy.TotalTrial > 0)
                    {
                        if (Data.MoveBackPercentAccuracy.FailureNeeded <= iFailedAccuracyCount)
                        {
                            res.MovedBackSet = true;
                        }//--- [New Criteria] May 2020 --|MoveBackPercentAccuracy|--(Start)-- //
                        else if (Data.MoveBackPercentAccuracy.ConsecutiveAverageFailure && (Data.MoveBackPercentAccuracy.FailureNeeded <= iConsecutiveAvgFailureAccuracyCount))
                        {
                            res.MovedBackSet = true;
                        }//--- [New Criteria] May 2020 --|MoveBackPercentAccuracy|--(End)-- //
                        else if (Data.MoveBackPercentAccuracy.ConsecutiveFailures && (Data.MoveBackPercentAccuracy.FailureNeeded <= iConsecutiveFailureAccuracyCount))
                        {
                            res.MovedBackSet = true;
                        }
                    }
                }
                else if (Data.NoPromptsUsed.Length > 0)
                {
                    if ((Data.sCurrentLessonPrompt == Data.NoPromptsUsed[0]) || (Data.promptDown >= 1)) // or prompt criteria is NA
                    {
                        if (Data.MoveBackPercentAccuracy.TotalTrial > 0)
                        {
                            if (Data.MoveBackPercentAccuracy.FailureNeeded <= iFailedAccuracyCount)
                            {
                                res.MovedBackSet = true;
                            }//--- [New Criteria] May 2020 --|MoveBackPercentAccuracy|--(Start)-- //
                            else if (Data.MoveBackPercentAccuracy.ConsecutiveAverageFailure && (Data.MoveBackPercentAccuracy.FailureNeeded <= iConsecutiveAvgFailureAccuracyCount))
                            {
                                res.MovedBackSet = true;
                            }//--- [New Criteria] May 2020 --|MoveBackPercentAccuracy|--(End)-- //
                            else if (Data.MoveBackPercentAccuracy.ConsecutiveFailures && (Data.MoveBackPercentAccuracy.FailureNeeded <= iConsecutiveFailureAccuracyCount))
                            {
                                res.MovedBackSet = true;
                            }
                        }
                    }

                }
                else
                {
                    if (Data.MoveBackPercentAccuracy.TotalTrial > 0)
                    {
                        if (Data.MoveBackPercentAccuracy.FailureNeeded <= iFailedAccuracyCount)
                        {
                            res.MovedBackSet = true;
                        }//--- [New Criteria] May 2020 --|MoveBackPercentAccuracy|--(Start)-- //
                        else if (Data.MoveBackPercentAccuracy.ConsecutiveAverageFailure && (Data.MoveBackPercentAccuracy.FailureNeeded <= iConsecutiveAvgFailureAccuracyCount))
                        {
                            res.MovedBackSet = true;
                        }//--- [New Criteria] May 2020 --|MoveBackPercentAccuracy|--(End)-- //
                        else if (Data.MoveBackPercentAccuracy.ConsecutiveFailures && (Data.MoveBackPercentAccuracy.FailureNeeded <= iConsecutiveFailureAccuracyCount))
                        {
                            res.MovedBackSet = true;
                        }
                    }
                }

                if (bpromptColumn)
                {
                    if (Data.MoveBackPercentIndependence.TotalTrial > 0)
                    {
                        if (Data.MoveBackPercentIndependence.FailureNeeded <= iFailedIndCount)
                        {
                            res.MovedBackSet = true;
                        }//--- [New Criteria] May 2020 --|MoveBackPercentIndependence|--(Start)-- //
                        else if (Data.MoveBackPercentIndependence.ConsecutiveAverageFailure && (Data.MoveBackPercentIndependence.FailureNeeded <= iConsecutiveAvgFailureIndCount))
                        {
                            res.MovedBackSet = true;
                        }//--- [New Criteria] May 2020 --|MoveBackPercentIndependence|--(End)-- //
                        else if (Data.MoveBackPercentIndependence.ConsecutiveFailures && (Data.MoveBackPercentIndependence.FailureNeeded <= iConsecutiveFailureIndCount))
                        {
                            res.MovedBackSet = true;
                        }
                    }
                    if (Data.SetTotalCorrectMoveBack.TotalTrial > 0)
                    {
                        if (Data.SetTotalCorrectMoveBack.FailureNeeded <= iFailedCorrectCount)
                        {
                            res.MovedBackSet = true;
                        }//--- [New Criteria] May 2020 --|SetTotalCorrectMoveBack|--(Start)-- //
                        else if (Data.SetTotalCorrectMoveBack.ConsecutiveAverageFailure && (Data.SetTotalCorrectMoveBack.FailureNeeded <= iConsecutiveAvgFailureCorrectCount))
                        {
                            res.MovedBackSet = true;
                        }//--- [New Criteria] May 2020 --|SetTotalCorrectMoveBack|--(End)-- //
                        else if (Data.SetTotalCorrectMoveBack.ConsecutiveFailures && (Data.SetTotalCorrectMoveBack.FailureNeeded <= iConsecutiveFailureCorrectCount))
                        {
                            res.MovedBackSet = true;
                        }
                    }
                    if (Data.SetTotalIncorrectMoveBack.TotalTrial > 0)
                    {
                        if (Data.SetTotalIncorrectMoveBack.FailureNeeded <= iInCorrectCount)
                        {
                            res.MovedBackSet = true;
                        }//--- [New Criteria] May 2020 --|SetTotalIncorrectMoveBack|--(Start)-- //
                        else if (Data.SetTotalIncorrectMoveBack.ConsecutiveAverageFailure && (Data.SetTotalIncorrectMoveBack.FailureNeeded <= iConsecutiveAvgFailureInCorrectCount))
                        {
                            res.MovedBackSet = true;
                        }//--- [New Criteria] May 2020 --|SetTotalIncorrectMoveBack|--(End)-- //
                        else if (Data.SetTotalIncorrectMoveBack.ConsecutiveFailures && (Data.SetTotalIncorrectMoveBack.FailureNeeded <= iConsecutiveFailureInCorrectCount))
                        {
                            res.MovedBackSet = true;
                        }
                    }
                }
                else if (Data.NoPromptsUsed.Length > 0)
                {
                    if ((Data.sCurrentLessonPrompt == Data.NoPromptsUsed[0]) || (Data.promptDown >= 1)) // or prompt criteria is NA
                    {
                        if (Data.MoveBackPercentIndependence.TotalTrial > 0)
                        {
                            if (Data.MoveBackPercentIndependence.FailureNeeded <= iFailedIndCount)
                            {
                                res.MovedBackSet = true;
                            }//--- [New Criteria] May 2020 --|MoveBackPercentIndependence|--(Start)-- //
                            else if (Data.MoveBackPercentIndependence.ConsecutiveAverageFailure && (Data.MoveBackPercentIndependence.FailureNeeded <= iConsecutiveAvgFailureIndCount))
                            {
                                res.MovedBackSet = true;
                            }//--- [New Criteria] May 2020 --|MoveBackPercentIndependence|--(End)-- //
                            else if (Data.MoveBackPercentIndependence.ConsecutiveFailures && (Data.MoveBackPercentIndependence.FailureNeeded <= iConsecutiveFailureIndCount))
                            {
                                res.MovedBackSet = true;
                            }
                        }
                        if (Data.SetTotalCorrectMoveBack.TotalTrial > 0)
                        {
                            if (Data.SetTotalCorrectMoveBack.FailureNeeded <= iFailedCorrectCount)
                            {
                                res.MovedBackSet = true;
                            }//--- [New Criteria] May 2020 --|SetTotalCorrectMoveBack|--(Start)-- //
                            else if (Data.SetTotalCorrectMoveBack.ConsecutiveAverageFailure && (Data.SetTotalCorrectMoveBack.FailureNeeded <= iConsecutiveAvgFailureCorrectCount))
                            {
                                res.MovedBackSet = true;
                            }//--- [New Criteria] May 2020 --|SetTotalCorrectMoveBack|--(End)-- //
                            else if (Data.SetTotalCorrectMoveBack.ConsecutiveFailures && (Data.SetTotalCorrectMoveBack.FailureNeeded <= iConsecutiveFailureCorrectCount))
                            {
                                res.MovedBackSet = true;
                            }
                        }
                        if (Data.SetTotalIncorrectMoveBack.TotalTrial > 0)
                        {
                            if (Data.SetTotalIncorrectMoveBack.FailureNeeded <= iInCorrectCount)
                            {
                                res.MovedBackSet = true;
                            }//--- [New Criteria] May 2020 --|SetTotalIncorrectMoveBack|--(Start)-- //
                            else if (Data.SetTotalIncorrectMoveBack.ConsecutiveAverageFailure && (Data.SetTotalIncorrectMoveBack.FailureNeeded <= iConsecutiveAvgFailureInCorrectCount))
                            {
                                res.MovedBackSet = true;
                            }//--- [New Criteria] May 2020 --|SetTotalIncorrectMoveBack|--(End)-- //
                            else if (Data.SetTotalIncorrectMoveBack.ConsecutiveFailures && (Data.SetTotalIncorrectMoveBack.FailureNeeded <= iConsecutiveFailureInCorrectCount))
                            {
                                res.MovedBackSet = true;
                            }
                        }
                    }
                }
                else
                {
                    if (Data.MoveBackPercentIndependence.TotalTrial > 0)
                    {
                        if (Data.MoveBackPercentIndependence.FailureNeeded <= iFailedIndCount)
                        {
                            res.MovedBackSet = true;
                        }//--- [New Criteria] May 2020 --|MoveBackPercentIndependence|--(Start)-- //
                        else if (Data.MoveBackPercentIndependence.ConsecutiveAverageFailure && (Data.MoveBackPercentIndependence.FailureNeeded <= iConsecutiveAvgFailureIndCount))
                        {
                            res.MovedBackSet = true;
                        }//--- [New Criteria] May 2020 --|MoveBackPercentIndependence|--(End)-- //
                        else if (Data.MoveBackPercentIndependence.ConsecutiveFailures && (Data.MoveBackPercentIndependence.FailureNeeded <= iConsecutiveFailureIndCount))
                        {
                            res.MovedBackSet = true;
                        }
                    }
                    if (Data.SetTotalCorrectMoveBack.TotalTrial > 0)
                    {
                        if (Data.SetTotalCorrectMoveBack.FailureNeeded <= iFailedCorrectCount)
                        {
                            res.MovedBackSet = true;
                        }//--- [New Criteria] May 2020 --|SetTotalCorrectMoveBack|--(Start)-- //
                        else if (Data.SetTotalCorrectMoveBack.ConsecutiveFailures && (Data.SetTotalCorrectMoveBack.FailureNeeded <= iConsecutiveAvgFailureCorrectCount))
                        {
                            res.MovedBackSet = true;
                        }//--- [New Criteria] May 2020 --|SetTotalCorrectMoveBack|--(End)-- //
                        else if (Data.SetTotalCorrectMoveBack.ConsecutiveFailures && (Data.SetTotalCorrectMoveBack.FailureNeeded <= iConsecutiveFailureCorrectCount))
                        {
                            res.MovedBackSet = true;
                        }
                    }
                    if (Data.SetTotalIncorrectMoveBack.TotalTrial > 0)
                    {
                        if (Data.SetTotalIncorrectMoveBack.FailureNeeded <= iInCorrectCount)
                        {
                            res.MovedBackSet = true;
                        }//--- [New Criteria] May 2020 --|SetTotalIncorrectMoveBack|--(Start)-- //
                        else if (Data.SetTotalIncorrectMoveBack.ConsecutiveFailures && (Data.SetTotalIncorrectMoveBack.FailureNeeded <= iConsecutiveAvgFailureInCorrectCount))
                        {
                            res.MovedBackSet = true;
                        }//--- [New Criteria] May 2020 --|SetTotalIncorrectMoveBack|--(Start)-- //
                        else if (Data.SetTotalIncorrectMoveBack.ConsecutiveFailures && (Data.SetTotalIncorrectMoveBack.FailureNeeded <= iConsecutiveFailureInCorrectCount))
                        {
                            res.MovedBackSet = true;
                        }
                    }
                }

                if (res.MovedBackSet)
                {
                    if (Data.CurrentSet > 1)
                        if (res.MovedBackPrompt)
                        {
                            res.MovedBackSet = false;
                        }
                        else
                            res.NextSet = Data.CurrentSet - 1;
                    else
                        res.NextSet = 1;
                }
            }


            if (res.MovedForwardSet)
            {
                if (Data.CurrentSet >= Data.TotalSets)
                {
                    res.CompletionStatus = "COMPLETED";
                    res.NextSet = Data.TotalSets;
                    return res;
                }
                //res.NextPrompt = Data.PromptsUsed[0];
                res.NextSet = Data.CurrentSet + 1;
            }
            //Return the new Prompt and SET


            return res;
        }
    }
}

//Is the IOA and multiteacher set at SET level or Lesson Plan level??


