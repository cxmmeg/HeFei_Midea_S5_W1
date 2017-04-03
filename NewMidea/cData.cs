using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
namespace NewMideaProgram
{
    static class cData
    {
        public static OleDbConnection ConnMain = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\\Data\\Main.mdb;Persist Security Info=True");
        public static OleDbConnection ConnData = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\\Data\\Data.mdb;Persist Security Info=True");

        public static bool SaveAnGuiData(string barcode, double[] data, bool[] value)
        {
            bool result = false;
            try
            {
                if (data != null && value != null && data.Length == 4 && value.Length == 4)
                {
                    result = (upData(string.Format("insert into AllAnGui (BarCode,d0,d1,d2,d3,b0,b1,b2,b3) values ('{0}',{1},{2},{3},{4},{5},{6},{7},{8})",
                        barcode, data[0], data[1], data[2], data[3], value[0], value[1], value[2], value[3]), ConnData) == 1);
                }
            }
            catch(Exception e)
            {
                All.Class.Error.Add(e);
            }
            return result;
        }

        public static bool SaveJianCeData(cNetResult mNetResult)
        {
            bool returnResult = false;
            int i;
            string sqlCommand = "Insert into AllData ({0}) values({1}{2}{3})";
            string tempStrHead = "#{0:yyyy-MM-dd HH:mm:ss}#,'{1}','{2}','{3}',{4},{5},'{6}',{7},'{8}'";
            string tempStrData = "";
            string tempStrBool = "";

            string strHead = "TestTime,Bar,ID,Mode,TestNo,JiQi,IsPass,StepId,Step";
            for (i = 0; i < 60; i++)
            {
                strHead = string.Format("{0},d{1}", strHead, i);
            }
            for (i = 0; i < 60; i++)
            {
                strHead = string.Format("{0},b{1}", strHead, i);
            }


            bool stepResult = true;
            try
            {
                if (mNetResult.StepResult.mIsStepPass == 0)
                {
                    stepResult = false;
                }
                tempStrHead = string.Format(tempStrHead,
                    mNetResult.RunResult.mTestTime,
                    mNetResult.RunResult.mBar,
                    mNetResult.RunResult.mId,
                    mNetResult.RunResult.mMode,
                    mNetResult.RunResult.mTestNo,
                    mNetResult.RunResult.mJiQi,
                    stepResult.ToString(),
                    mNetResult.RunResult.mStepId,
                    mNetResult.RunResult.mStep);
                for (i = 0; i < 60; i++)
                {
                    if (i < cMain.DataShow)
                    {
                        tempStrData = tempStrData + "," + mNetResult.StepResult.mData[i].ToString();
                        if (mNetResult.StepResult.mIsDataPass[i] == 0)
                        {
                            tempStrBool = tempStrBool + ",false";
                        }
                        else
                        {
                            tempStrBool = tempStrBool + ",true";
                        }
                    }
                    else
                    {
                        tempStrData = tempStrData + ",0";
                        tempStrBool = tempStrBool + ",true";
                    }
                }
                sqlCommand = string.Format(sqlCommand,strHead, tempStrHead, tempStrData, tempStrBool);
                if (upData(sqlCommand, ConnData) > 0)
                {
                    returnResult = true;
                }
                else
                {
                    returnResult = false;
                }
            }
            catch (Exception exc)
            {
                cMain.WriteErrorToLog("cData SaveJianCeData is Error " + exc.ToString());
                returnResult = false;
            }
            return returnResult;
        }
        public static DataSet readData(string sqlCommand, OleDbConnection conn)
        {
            DataSet ds = new DataSet();
            lock (conn)
            {
                OleDbCommand cmd = new OleDbCommand(sqlCommand, conn);
                OleDbDataAdapter oda = new OleDbDataAdapter(cmd);
                oda.Fill(ds);
                return ds;
            }
        }
        public static int upData(string sqlCommand, OleDbConnection conn)
        {
            int x = 0;
            try
            {
                lock (conn)
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    OleDbCommand cmd = new OleDbCommand(sqlCommand, conn);
                    cmd.CommandType = CommandType.Text;
                    x = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception exc)
            {
                cMain.WriteErrorToLog("cData upData is Error " + sqlCommand + "\r\n" + exc.ToString());
                x = 0;
            }
            return x;//返回受影响的行数
        }
    }

}
