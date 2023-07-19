using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;


namespace DiscreetTrial
{

    public class DbLogics
    {

    }

    public class SessionData
    {
        
        SqlConnection con = new SqlConnection("Data Source=192.168.1.8;Initial Catalog=AppDataP2;Persist Security Info=True;User ID=dev;Password=test");

        static int templateId = 0;
        public int sessionCount = 0;
        public int trialsCount = 0;
        public int totalSet = 0;

        public ArrayList GetTrialLists(int studentId, int templateID, int nmbrOfSession, string colName)
        {
            templateId = templateID;

            ArrayList Trials = new ArrayList();

            string sql = "SELECT S.StdtSessionStepId,S.StepVal, SS.SessionStatusCd, SS.StdtSessionHdrId, S.DSTempSetColId,DCol.ColName" +
                         " FROM StdtSessionDtl S" +
                         " INNER JOIN StdtSessionStep SS" +
                          " ON S.StdtSessionStepId = SS.StdtSessionStepId" +
                         " INNER JOIN StdtSessionHdr SH" +
                          " on SS.StdtSessionHdrId = sh.StdtSessionHdrId" +
                         " INNER JOIN DSTempSetCol DCol" +
                          " ON S.DSTempSetColId = DCol.DSTempSetColId" +
                         " WHERE SH.StdtSessionHdrId IN (select StdtSessionHdrId from ( select hdr.StdtSessionHdrId, RANK()" +
                         " OVER (ORDER BY EndTs DESC) as RNK  FROM StdtSessionHdr hdr" +
                         " WHERE StudentId = 8 AND DSTempHdrId = 2) as Rk WHERE RNK <= " + nmbrOfSession + ")   AND DCol.ColName = '" + colName + "' ORDER BY SS.StdtSessionHdrId";

            string line = "";

            SqlCommand command = new SqlCommand(sql, con);

            con.Open();

            SqlDataReader reader = command.ExecuteReader();

            int previousHdrId = 0;

            sessionCount = 0;

            while (reader.Read())
            {
                int hdrId = Convert.ToInt32(reader["StdtSessionHdrId"].ToString());
                if (hdrId != previousHdrId)
                {
                    line = "Trial,Score,Duration,Mistrial";
                    Trials.Add(line);
                    sessionCount++;
                }
                else
                {
                    line = reader["StdtSessionStepId"].ToString() + "," + reader["StepVal"].ToString() + "," + reader["SessionStatusCd"].ToString() + ",10,";
                    Trials.Add(line);
                    trialsCount++;
                }

                previousHdrId = Convert.ToInt32(reader["StdtSessionHdrId"].ToString());
            }
            trialsCount = trialsCount / sessionCount;

            reader.Close();

            sql = "select count(DSTempSetId) from DSTempSet where DSTempHdrId=" + templateID;

            command = new SqlCommand(sql, con);

            totalSet = Convert.ToInt32(command.ExecuteScalar());

            con.Close();

            return Trials;
        }


        public String[] GetPrompt(int tempId)
        {

            con.Open();

            templateId = tempId;

            string[] prompt = null;

            int index = 0;

            string sql = "select PromptId from DSTempPrompt where DSTempHdrId=" + tempId + " ORDER BY PromptOrder";

            SqlCommand command = new SqlCommand(sql, con);

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {

                prompt[index] = reader["PromptId"].ToString();

                index++;

            }

            reader.Close();

            con.Close();
           
            return prompt;

        }

        public int SaveStatus(int nextSet, string nextPrompt)
        {
            int i = 0;

            con.Open();

            string qry = "INSERT INTO StdtDSStat (SchoolId ,StudentId ,DSTempHdrId ,LessonPlanId ,NextSetId ,NextStepId ,NextPromptId ,NextSessionNbr ,CreatedBy" +
                " ,CreatedOn ) VALUES (1,8," + templateId + ",20," + nextSet + ",1,'" + nextPrompt + "',1,1,GETDATE())";

            SqlCommand cmd = new SqlCommand(qry, con);

            i = cmd.ExecuteNonQuery();

            con.Close();

            return i;
        }


        public void updateSetStatus(string nextSet)
        {

            con.Open();

            string qry = "UPDATE StdtDSStat SET NextSetId='" + nextSet + "' ,ModifiedBy=1 ,ModifiedOn=GETDATE()" +

                " WHERE DSTempHdrId=" + templateId + "";


            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();

            con.Close();


        }
        
        public void updatePromptStatus(string nextPrompt)
        {
            
            con.Open();

            string qry = "UPDATE StdtDSStat SET NextPromptId='" + nextPrompt + "' ,ModifiedBy=1 ,ModifiedOn=GETDATE()" +

                " WHERE DSTempHdrId=" + templateId + "";


            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();

            con.Close();
        }


    }
}

