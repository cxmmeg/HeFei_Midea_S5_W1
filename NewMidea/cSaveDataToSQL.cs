using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
namespace NewMideaProgram
{
    public class cSaveDataToSQL
    {
        const string fileName = "DataIndex.txt";
        long testIndex = 0;
        long anguiIndex = 0;
        All.Class.DataReadAndWrite SQLServer
        { get; set; }
        /// <summary>
        /// 实时数据
        /// </summary>
        public cNetResult TmpResult
        { get; set; }
        public cSaveDataToSQL()
        {
            TmpResult = new cNetResult();
            Load();
            new Thread(() => FlushDataToRemot())
            {
                IsBackground = true
            }.Start();
        }
        private void Load()
        {
            if (System.IO.File.Exists(string.Format("{0}\\Data\\{1}", All.Class.FileIO.GetNowPath(), fileName)))
            {
                Dictionary<string, string> buff = All.Class.SSFile.Text2Dictionary(
                    All.Class.FileIO.ReadFile(string.Format("{0}\\Data\\{1}", All.Class.FileIO.GetNowPath(), fileName)));
                if (buff.ContainsKey("Index"))
                    testIndex = All.Class.Num.ToLong(buff["Index"]); 
                if (buff.ContainsKey("AnGuiIndex"))
                    anguiIndex = All.Class.Num.ToLong(buff["AnGuiIndex"]);
            }
            Save();
        }
        private void Save()
        {
            Dictionary<string,string> buff=new Dictionary<string,string>();
            buff.Add("Index",testIndex.ToString());
            buff.Add("AnGuiIndex", anguiIndex.ToString());
            All.Class.FileIO.Write(string.Format("{0}\\Data\\{1}", All.Class.FileIO.GetNowPath(), fileName),
                All.Class.SSFile.Dictionary2Text(buff), System.IO.FileMode.Create);
        }
        private void FlushDataToRemot()
        {
            while (true)
            {
                if (Login())
                {
                    if (TmpResult != null)
                    {
                            string sql = "update AllTestDataTmp Set {0}{1}{2} where DataID={3}";
                            string tempStrHead = "TestTime='{0:yyyy-MM-dd HH:mm:ss}',Bar='{1}',Id='{2}',Mode='{3}',TestNo={4},JiQi={5},IsPass='{6}',StepId={7},Step='{8}'";
                            string tempStrData = "";
                            string tempStrBool = "";
                            tempStrHead = string.Format(tempStrHead, DateTime.Now,
                                                                 TmpResult.RunResult.mBar,
                                                                 TmpResult.RunResult.mId,
                                                                 TmpResult.RunResult.mMode,
                                                                 TmpResult.RunResult.mTestNo,
                                                                 TmpResult.RunResult.mJiQi,
                                                                 TmpResult.RunResult.mIsPass,
                                                                 TmpResult.RunResult.mStepId,
                                                                 TmpResult.RunResult.mStep);
                            for (int i = 0; i < cMain.DataShow; i++)
                            {
                                tempStrData = string.Format("{0},d{1}={2:F3}", tempStrData, i, TmpResult.StepResult.mData[i]);
                                tempStrBool = string.Format("{0},b{1}='{2}'", tempStrBool, i, TmpResult.StepResult.mIsDataPass[i] == 1);
                            }
                            for (int i = cMain.DataShow; i < 60; i++)
                            {
                                tempStrData = string.Format("{0},d{1}={2:F3}", tempStrData, i, 0);
                                tempStrBool = string.Format("{0},b{1}='{1}'", tempStrBool, i, true);
                            }
                            sql = string.Format(sql, tempStrHead, tempStrData, tempStrBool, cMain.ThisNo - 160);
                            SQLServer.Write(sql);
                    }

                    SQLServer.Write(string.Format("update AllTestStatue Set RandomValue={0},NowStatue={1},Plc='{2}',Ft2010='{3}',Mod7017_1='{4}',Mod7017_2='{5}',Mod7017_3='{6}',Mod7017_4='{7}',Angui7440='{8}',Angui7623='{9}',ChouKong='{10}' where TestNo={11}",
                        (int)All.Class.Num.GetRandom(0, 100), frmMain.NowStatue, frmMain.Plc, frmMain.Ft2010, frmMain.Mod7017_1, frmMain.Mod7017_2, frmMain.Mod7017_3, frmMain.Mod7017_4, frmMain.AnGui7440, frmMain.AnGui7623, frmMain.ChouKong, cMain.ThisNo - 160));

                    using (DataSet ds = cData.readData(string.Format("select Top 1 * from AllAnGui Where DataID>{0} order by DataID", anguiIndex), cData.ConnData))
                    {
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            if (SQLServer.Write(string.Format("insert into AllTestAnGui Values('{0}',{1},{2},{3},{4},'{5}','{6}','{7}','{8}')",
                                ds.Tables[0].Rows[0]["BarCode"],
                                ds.Tables[0].Rows[0]["d0"],
                                ds.Tables[0].Rows[0]["d1"],
                                ds.Tables[0].Rows[0]["d2"],
                                ds.Tables[0].Rows[0]["d3"],
                                ds.Tables[0].Rows[0]["b0"],
                                ds.Tables[0].Rows[0]["b1"],
                                ds.Tables[0].Rows[0]["b2"],
                                ds.Tables[0].Rows[0]["b3"])) == 1)
                            {
                                anguiIndex++;
                                Save();
                            }
                        }
                    }
                    using (DataSet ds = cData.readData(string.Format("select Top 1 * from AllData Where DataID>{0} order by DataID", testIndex),cData.ConnData))
                    {
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            string sql = "insert into AllTestData Values({0}{1}{2})";
                            string tempStrHead = "'{0:yyyy-MM-dd HH:mm:ss}','{1}','{2}','{3}',{4},{5},'{6}',{7},'{8}'";
                            string tempStrData = "";
                            string tempStrBool = "";
                            tempStrHead = string.Format(tempStrHead, ds.Tables[0].Rows[0]["TestTime"],
                                                                 ds.Tables[0].Rows[0]["Bar"],
                                                                 ds.Tables[0].Rows[0]["ID"],
                                                                 ds.Tables[0].Rows[0]["Mode"],
                                                                 ds.Tables[0].Rows[0]["TestNo"],
                                                                 ds.Tables[0].Rows[0]["JiQi"],
                                                                 ds.Tables[0].Rows[0]["IsPass"],
                                                                 ds.Tables[0].Rows[0]["StepId"],
                                                                 ds.Tables[0].Rows[0]["Step"]);
                            for (int i = 0; i < 60; i++)
                            {
                                tempStrData = string.Format("{0},{1:F3}", tempStrData,All.Class.Num.ToFloat( ds.Tables[0].Rows[0][string.Format("d{0}", i)]));
                                tempStrBool = string.Format("{0},'{1}'", tempStrBool, All.Class.Num.ToBool( ds.Tables[0].Rows[0][string.Format("b{0}", i)]));
                            }
                            sql = string.Format(sql, tempStrHead, tempStrData, tempStrBool);
                            if (SQLServer.Write(sql) == 1)
                            {
                                testIndex++;
                                Save();
                            }
                        }
                    }

                }
                Thread.Sleep(500);
            }
        }

        private bool Login()
        {
            if (SQLServer == null || SQLServer.Conn.State != System.Data.ConnectionState.Open)
            {
                SQLServer = All.Class.DataReadAndWrite.GetData(string.Format("{0}\\Data\\DataConnect.txt", All.Class.FileIO.GetNowPath()), "LocalData");
            }
            return (SQLServer != null && SQLServer.Conn.State == System.Data.ConnectionState.Open);
        }
        //public bool SaveRemotJianCeData(cNetResult mNetResult)
        //{
        //    bool result = false;

        //    string sql = "insert into AllData Values({0}{1}{2})";
        //    string tempStrHead = "'{0:yyyy-MM-dd HH:mm:ss}','{1}','{2}','{3}',{4},{5},'{6}',{7},'{8}'";
        //    string tempStrData = "";
        //    string tempStrBool = "";
        //    bool stepResult = true;
        //    try
        //    {
        //        if (mNetResult.StepResult.mIsStepPass == 0)
        //        {
        //            stepResult = false;
        //        }
        //        tempStrHead = string.Format(tempStrHead,
        //            mNetResult.RunResult.mTestTime,
        //            mNetResult.RunResult.mBar,
        //            mNetResult.RunResult.mId,
        //            mNetResult.RunResult.mMode,
        //            mNetResult.RunResult.mTestNo,
        //            mNetResult.RunResult.mJiQi,
        //            stepResult.ToString(),
        //            mNetResult.RunResult.mStepId,
        //            mNetResult.RunResult.mStep);
        //        for (int i = 0; i < 60; i++)
        //        {
        //            if (i < cMain.DataShow)
        //            {
        //                tempStrData = tempStrData + "," + mNetResult.StepResult.mData[i].ToString();
        //                if (mNetResult.StepResult.mIsDataPass[i] == 0)
        //                {
        //                    tempStrBool = tempStrBool + ",false";
        //                }
        //                else
        //                {
        //                    tempStrBool = tempStrBool + ",true";
        //                }
        //            }
        //            else
        //            {
        //                tempStrData = tempStrData + ",0";
        //                tempStrBool = tempStrBool + ",true";
        //            }
        //        }
        //        sql = string.Format(sql, tempStrHead, tempStrData, tempStrBool);
        //        result = (SQLServer.Write(sql) == 1);
        //    }
        //    catch (Exception exc)
        //    {
        //        All.Class.Error.Add(exc);
        //    }
        //    return result;
        //}
    }
}
