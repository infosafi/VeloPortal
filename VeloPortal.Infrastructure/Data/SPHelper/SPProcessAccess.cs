using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloPortal.Domain.Extensions;

namespace VeloPortal.Infrastructure.Data.SPHelper
{
    public class SPProcessAccess
    {
        SPDataAccess? _dataAccess;

        public SPProcessAccess(string? connectionsring)
        {
            if (connectionsring != null)
            {
                _dataAccess = new SPDataAccess(connectionsring);

            }

        }


        public DataSet? GetTransInfo20(string comCode, string SQLprocName, string CallType, string mDesc1 = "", string mDesc2 = "",
 string mDesc3 = "", string mDesc4 = "", string mDesc5 = "", string mDesc6 = "", string mDesc7 = "",
 string mDesc8 = "", string mDesc9 = "", string mDesc10 = "", string mDesc11 = "",
 string mDesc12 = "", string mDesc13 = "", string mDesc14 = "", string mDesc15 = "",
     string mDesc16 = "", string mDesc17 = "", string mDesc18 = "", string mDesc19 = "", string mDesc20 = "",
     string @userid = "")
        {
            try
            {
                ErrorTrackingExtension.ClearErrors();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SQLprocName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Comcod", comCode));
                cmd.Parameters.Add(new SqlParameter("@CallType", CallType));
                cmd.Parameters.Add(new SqlParameter("@Desc1", mDesc1));
                cmd.Parameters.Add(new SqlParameter("@Desc2", mDesc2));
                cmd.Parameters.Add(new SqlParameter("@Desc3", mDesc3));
                cmd.Parameters.Add(new SqlParameter("@Desc4", mDesc4));
                cmd.Parameters.Add(new SqlParameter("@Desc5", mDesc5));
                cmd.Parameters.Add(new SqlParameter("@Desc6", mDesc6));
                cmd.Parameters.Add(new SqlParameter("@Desc7", mDesc7));
                cmd.Parameters.Add(new SqlParameter("@Desc8", mDesc8));
                cmd.Parameters.Add(new SqlParameter("@Desc9", mDesc9));
                cmd.Parameters.Add(new SqlParameter("@Desc10", mDesc10));
                cmd.Parameters.Add(new SqlParameter("@Desc11", mDesc11));
                cmd.Parameters.Add(new SqlParameter("@Desc12", mDesc12));
                cmd.Parameters.Add(new SqlParameter("@Desc13", mDesc13));
                cmd.Parameters.Add(new SqlParameter("@Desc14", mDesc14));
                cmd.Parameters.Add(new SqlParameter("@Desc15", mDesc15));
                cmd.Parameters.Add(new SqlParameter("@Desc16", mDesc16));
                cmd.Parameters.Add(new SqlParameter("@Desc17", mDesc17));
                cmd.Parameters.Add(new SqlParameter("@Desc18", mDesc18));
                cmd.Parameters.Add(new SqlParameter("@Desc19", mDesc19));
                cmd.Parameters.Add(new SqlParameter("@Desc20", mDesc20));
                cmd.Parameters.Add(new SqlParameter("@UserID", @userid));
                if (_dataAccess == null)
                {
                    ErrorTrackingExtension.SetError(new NullReferenceException("_dataAccess is null."));
                    return null;
                }

                DataSet? result = _dataAccess.GetDataSet(cmd);

                return result;
            }
            catch (Exception exp)
            {
                ErrorTrackingExtension.SetError(exp);
                return null;
            }
        }

        public bool UpdateTransInf20(string comCode, string SQLprocName, string CallType,
     string mDesc1 = "", string mDesc2 = "", string mDesc3 = "", string mDesc4 = "", string mDesc5 = "", string mDesc6 = "",
     string mDesc7 = "", string mDesc8 = "", string mDesc9 = "", string mDesc10 = "", string mDesc11 = "", string mDesc12 = "",
     string mDesc13 = "", string mDesc14 = "", string mDesc15 = "", string mDesc16 = "", string mDesc17 = "", string mDesc18 = "",
    string mDesc19 = "", string mDesc20 = "")
        {
            try
            {
                ErrorTrackingExtension.ClearErrors();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SQLprocName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Comcod", comCode));
                cmd.Parameters.Add(new SqlParameter("@CallType", CallType));
                cmd.Parameters.Add(new SqlParameter("@Desc1", mDesc1));
                cmd.Parameters.Add(new SqlParameter("@Desc2", mDesc2));
                cmd.Parameters.Add(new SqlParameter("@Desc3", mDesc3));
                cmd.Parameters.Add(new SqlParameter("@Desc4", mDesc4));
                cmd.Parameters.Add(new SqlParameter("@Desc5", mDesc5));
                cmd.Parameters.Add(new SqlParameter("@Desc6", mDesc6));
                cmd.Parameters.Add(new SqlParameter("@Desc7", mDesc7));
                cmd.Parameters.Add(new SqlParameter("@Desc8", mDesc8));
                cmd.Parameters.Add(new SqlParameter("@Desc9", mDesc9));
                cmd.Parameters.Add(new SqlParameter("@Desc10", mDesc10));
                cmd.Parameters.Add(new SqlParameter("@Desc11", mDesc11));
                cmd.Parameters.Add(new SqlParameter("@Desc12", mDesc12));
                cmd.Parameters.Add(new SqlParameter("@Desc13", mDesc13));
                cmd.Parameters.Add(new SqlParameter("@Desc14", mDesc14));
                cmd.Parameters.Add(new SqlParameter("@Desc15", mDesc15));
                cmd.Parameters.Add(new SqlParameter("@Desc16", mDesc16));
                cmd.Parameters.Add(new SqlParameter("@Desc17", mDesc17));
                cmd.Parameters.Add(new SqlParameter("@Desc18", mDesc18));
                cmd.Parameters.Add(new SqlParameter("@Desc19", mDesc19));
                cmd.Parameters.Add(new SqlParameter("@Desc20", mDesc20));
                if (_dataAccess == null)
                {
                    ErrorTrackingExtension.SetError(new NullReferenceException("_dataAccess is null."));
                    return false;
                }
                bool _result = _dataAccess.ExecuteCommand(cmd);

                return _result;
            }
            catch (Exception exp)
            {
                ErrorTrackingExtension.SetError(exp);
                return false;
            }// try
        }

        public bool UpdateTransInfo20(string comCode, string SQLprocName, string CallType,
        string mDesc1 = "", string mDesc2 = "", string mDesc3 = "", string mDesc4 = "", string mDesc5 = "", string mDesc6 = "",
        string mDesc7 = "", string mDesc8 = "", string mDesc9 = "", string mDesc10 = "", string mDesc11 = "", string mDesc12 = "",
        string mDesc13 = "", string mDesc14 = "", string mDesc15 = "", string mDesc16 = "", string mDesc17 = "", string mDesc18 = "",
        string mDesc19 = "", string mDesc20 = "", string mDesc21 = "", string mDesc22 = "", string mDesc23 = "", string mDesc24 = "",
        string mDesc25 = "", string mDesc26 = "", string mDesc27 = "", string mDesc28 = "", string mDesc29 = "", string mDesc30 = "",
        string mDesc31 = "", string mDesc32 = "", string mDesc33 = "", string mDesc34 = "", string mDesc35 = "", string mDesc36 = "",
        string mDesc37 = "", string mDesc38 = "", string mDesc39 = "", string mDesc40 = "", string mDesc41 = "", string mDesc42 = "",
        string mDesc43 = "", string mDesc44 = "", string mDesc45 = "", string mDesc46 = "", string mDesc47 = "", string mDesc48 = "",
        string mDesc49 = "", string mDesc50 = "")
        {
            try
            {
                ErrorTrackingExtension.ClearErrors();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SQLprocName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Comcod", comCode));
                cmd.Parameters.Add(new SqlParameter("@CallType", CallType));
                cmd.Parameters.Add(new SqlParameter("@Desc1", mDesc1));
                cmd.Parameters.Add(new SqlParameter("@Desc2", mDesc2));
                cmd.Parameters.Add(new SqlParameter("@Desc3", mDesc3));
                cmd.Parameters.Add(new SqlParameter("@Desc4", mDesc4));
                cmd.Parameters.Add(new SqlParameter("@Desc5", mDesc5));
                cmd.Parameters.Add(new SqlParameter("@Desc6", mDesc6));
                cmd.Parameters.Add(new SqlParameter("@Desc7", mDesc7));
                cmd.Parameters.Add(new SqlParameter("@Desc8", mDesc8));
                cmd.Parameters.Add(new SqlParameter("@Desc9", mDesc9));
                cmd.Parameters.Add(new SqlParameter("@Desc10", mDesc10));
                cmd.Parameters.Add(new SqlParameter("@Desc11", mDesc11));
                cmd.Parameters.Add(new SqlParameter("@Desc12", mDesc12));
                cmd.Parameters.Add(new SqlParameter("@Desc13", mDesc13));
                cmd.Parameters.Add(new SqlParameter("@Desc14", mDesc14));
                cmd.Parameters.Add(new SqlParameter("@Desc15", mDesc15));
                cmd.Parameters.Add(new SqlParameter("@Desc16", mDesc16));
                cmd.Parameters.Add(new SqlParameter("@Desc17", mDesc17));
                cmd.Parameters.Add(new SqlParameter("@Desc18", mDesc18));
                cmd.Parameters.Add(new SqlParameter("@Desc19", mDesc19));
                cmd.Parameters.Add(new SqlParameter("@Desc20", mDesc20));
                cmd.Parameters.Add(new SqlParameter("@Desc21", mDesc21));
                cmd.Parameters.Add(new SqlParameter("@Desc22", mDesc22));
                cmd.Parameters.Add(new SqlParameter("@Desc23", mDesc23));
                cmd.Parameters.Add(new SqlParameter("@Desc24", mDesc24));
                cmd.Parameters.Add(new SqlParameter("@Desc25", mDesc25));
                cmd.Parameters.Add(new SqlParameter("@Desc26", mDesc26));
                cmd.Parameters.Add(new SqlParameter("@Desc27", mDesc27));
                cmd.Parameters.Add(new SqlParameter("@Desc28", mDesc28));
                cmd.Parameters.Add(new SqlParameter("@Desc29", mDesc29));
                cmd.Parameters.Add(new SqlParameter("@Desc30", mDesc30));
                cmd.Parameters.Add(new SqlParameter("@Desc31", mDesc31));
                cmd.Parameters.Add(new SqlParameter("@Desc32", mDesc32));
                cmd.Parameters.Add(new SqlParameter("@Desc33", mDesc33));
                cmd.Parameters.Add(new SqlParameter("@Desc34", mDesc34));
                cmd.Parameters.Add(new SqlParameter("@Desc35", mDesc35));
                cmd.Parameters.Add(new SqlParameter("@Desc36", mDesc36));
                cmd.Parameters.Add(new SqlParameter("@Desc37", mDesc37));
                cmd.Parameters.Add(new SqlParameter("@Desc38", mDesc38));
                cmd.Parameters.Add(new SqlParameter("@Desc39", mDesc39));
                cmd.Parameters.Add(new SqlParameter("@Desc40", mDesc40));
                cmd.Parameters.Add(new SqlParameter("@Desc41", mDesc41));
                cmd.Parameters.Add(new SqlParameter("@Desc42", mDesc42));
                cmd.Parameters.Add(new SqlParameter("@Desc43", mDesc43));
                cmd.Parameters.Add(new SqlParameter("@Desc44", mDesc44));
                cmd.Parameters.Add(new SqlParameter("@Desc45", mDesc45));
                cmd.Parameters.Add(new SqlParameter("@Desc46", mDesc46));
                cmd.Parameters.Add(new SqlParameter("@Desc47", mDesc47));
                cmd.Parameters.Add(new SqlParameter("@Desc48", mDesc48));
                cmd.Parameters.Add(new SqlParameter("@Desc49", mDesc49));
                cmd.Parameters.Add(new SqlParameter("@Desc50", mDesc50));
                if (_dataAccess == null)
                {
                    ErrorTrackingExtension.SetError(new NullReferenceException("_dataAccess is null."));
                    return false;
                }
                bool _result = _dataAccess.ExecuteCommand(cmd);

                return _result;
            }
            catch (Exception exp)
            {
                ErrorTrackingExtension.SetError(exp);
                return false;
            }// try
        }

        public bool UpdateTransInfo100(string comCode, string SQLprocName, string CallType,
          string mDesc1 = "", string mDesc2 = "", string mDesc3 = "", string mDesc4 = "", string mDesc5 = "", string mDesc6 = "",
          string mDesc7 = "", string mDesc8 = "", string mDesc9 = "", string mDesc10 = "", string mDesc11 = "", string mDesc12 = "",
          string mDesc13 = "", string mDesc14 = "",
           string mDesc15 = "", string mDesc16 = "", string mDesc17 = "", string mDesc18 = "",
          string mDesc19 = "", string mDesc20 = "", string mDesc21 = "", string mDesc22 = "", string mDesc23 = "", string mDesc24 = "",
          string mDesc25 = "", string mDesc26 = "", string mDesc27 = "", string mDesc28 = "", string mDesc29 = "", string mDesc30 = "",
          string mDesc31 = "", string mDesc32 = "", string mDesc33 = "", string mDesc34 = "", string mDesc35 = "", string mDesc36 = "",
          string mDesc37 = "", string mDesc38 = "", string mDesc39 = "", string mDesc40 = "", string mDesc41 = "", string mDesc42 = "",
          string mDesc43 = "", string mDesc44 = "", string mDesc45 = "", string mDesc46 = "", string mDesc47 = "", string mDesc48 = "",
          string mDesc49 = "", string mDesc50 = "", string mDesc51 = "", string mDesc52 = "", string mDesc53 = "", string mDesc54 = "",
          string mDesc55 = "", string mDesc56 = "", string mDesc57 = "", string mDesc58 = "", string mDesc59 = "", string mDesc60 = "",
          string mDesc61 = "", string mDesc62 = "", string mDesc63 = "", string mDesc64 = "", string mDesc65 = "", string mDesc66 = "",
          string mDesc67 = "", string mDesc68 = "", string mDesc69 = "", string mDesc70 = "", string mDesc71 = "", string mDesc72 = "",
          string mDesc73 = "", string mDesc74 = "", string mDesc75 = "", string mDesc76 = "", string mDesc77 = "", string mDesc78 = "",
          string mDesc79 = "", string mDesc80 = "", string mDesc81 = "", string mDesc82 = "", string mDesc83 = "", string mDesc84 = "",
          string mDesc85 = "", string mDesc86 = "", string mDesc87 = "", string mDesc88 = "", string mDesc89 = "", string mDesc90 = "",
          string mDesc91 = "", string mDesc92 = "", string mDesc93 = "", string mDesc94 = "", string mDesc95 = "", string mDesc96 = "",
          string mDesc97 = "", string mDesc98 = "", string mDesc99 = "", string mDesc100 = "", string mUserID = "")
        {
            try
            {
                ErrorTrackingExtension.ClearErrors();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SQLprocName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Comcod", comCode));
                cmd.Parameters.Add(new SqlParameter("@CallType", CallType));
                cmd.Parameters.Add(new SqlParameter("@Desc1", mDesc1));
                cmd.Parameters.Add(new SqlParameter("@Desc2", mDesc2));
                cmd.Parameters.Add(new SqlParameter("@Desc3", mDesc3));
                cmd.Parameters.Add(new SqlParameter("@Desc4", mDesc4));
                cmd.Parameters.Add(new SqlParameter("@Desc5", mDesc5));
                cmd.Parameters.Add(new SqlParameter("@Desc6", mDesc6));
                cmd.Parameters.Add(new SqlParameter("@Desc7", mDesc7));
                cmd.Parameters.Add(new SqlParameter("@Desc8", mDesc8));
                cmd.Parameters.Add(new SqlParameter("@Desc9", mDesc9));
                cmd.Parameters.Add(new SqlParameter("@Desc10", mDesc10));
                cmd.Parameters.Add(new SqlParameter("@Desc11", mDesc11));
                cmd.Parameters.Add(new SqlParameter("@Desc12", mDesc12));
                cmd.Parameters.Add(new SqlParameter("@Desc13", mDesc13));
                cmd.Parameters.Add(new SqlParameter("@Desc14", mDesc14));
                cmd.Parameters.Add(new SqlParameter("@Desc15", mDesc15));
                cmd.Parameters.Add(new SqlParameter("@Desc16", mDesc16));
                cmd.Parameters.Add(new SqlParameter("@Desc17", mDesc17));
                cmd.Parameters.Add(new SqlParameter("@Desc18", mDesc18));
                cmd.Parameters.Add(new SqlParameter("@Desc19", mDesc19));
                cmd.Parameters.Add(new SqlParameter("@Desc20", mDesc20));
                cmd.Parameters.Add(new SqlParameter("@Desc21", mDesc21));
                cmd.Parameters.Add(new SqlParameter("@Desc22", mDesc22));
                cmd.Parameters.Add(new SqlParameter("@Desc23", mDesc23));
                cmd.Parameters.Add(new SqlParameter("@Desc24", mDesc24));
                cmd.Parameters.Add(new SqlParameter("@Desc25", mDesc25));
                cmd.Parameters.Add(new SqlParameter("@Desc26", mDesc26));
                cmd.Parameters.Add(new SqlParameter("@Desc27", mDesc27));
                cmd.Parameters.Add(new SqlParameter("@Desc28", mDesc28));
                cmd.Parameters.Add(new SqlParameter("@Desc29", mDesc29));
                cmd.Parameters.Add(new SqlParameter("@Desc30", mDesc30));
                cmd.Parameters.Add(new SqlParameter("@Desc31", mDesc31));
                cmd.Parameters.Add(new SqlParameter("@Desc32", mDesc32));
                cmd.Parameters.Add(new SqlParameter("@Desc33", mDesc33));
                cmd.Parameters.Add(new SqlParameter("@Desc34", mDesc34));
                cmd.Parameters.Add(new SqlParameter("@Desc35", mDesc35));
                cmd.Parameters.Add(new SqlParameter("@Desc36", mDesc36));
                cmd.Parameters.Add(new SqlParameter("@Desc37", mDesc37));
                cmd.Parameters.Add(new SqlParameter("@Desc38", mDesc38));
                cmd.Parameters.Add(new SqlParameter("@Desc39", mDesc39));
                cmd.Parameters.Add(new SqlParameter("@Desc40", mDesc40));
                cmd.Parameters.Add(new SqlParameter("@Desc41", mDesc41));
                cmd.Parameters.Add(new SqlParameter("@Desc42", mDesc42));
                cmd.Parameters.Add(new SqlParameter("@Desc43", mDesc43));
                cmd.Parameters.Add(new SqlParameter("@Desc44", mDesc44));
                cmd.Parameters.Add(new SqlParameter("@Desc45", mDesc45));
                cmd.Parameters.Add(new SqlParameter("@Desc46", mDesc46));
                cmd.Parameters.Add(new SqlParameter("@Desc47", mDesc47));
                cmd.Parameters.Add(new SqlParameter("@Desc48", mDesc48));
                cmd.Parameters.Add(new SqlParameter("@Desc49", mDesc49));
                cmd.Parameters.Add(new SqlParameter("@Desc50", mDesc50));
                cmd.Parameters.Add(new SqlParameter("@Desc51", mDesc51));
                cmd.Parameters.Add(new SqlParameter("@Desc52", mDesc52));
                cmd.Parameters.Add(new SqlParameter("@Desc53", mDesc53));
                cmd.Parameters.Add(new SqlParameter("@Desc54", mDesc54));
                cmd.Parameters.Add(new SqlParameter("@Desc55", mDesc55));
                cmd.Parameters.Add(new SqlParameter("@Desc56", mDesc56));
                cmd.Parameters.Add(new SqlParameter("@Desc57", mDesc57));
                cmd.Parameters.Add(new SqlParameter("@Desc58", mDesc58));
                cmd.Parameters.Add(new SqlParameter("@Desc59", mDesc59));
                cmd.Parameters.Add(new SqlParameter("@Desc60", mDesc60));
                cmd.Parameters.Add(new SqlParameter("@Desc61", mDesc61));
                cmd.Parameters.Add(new SqlParameter("@Desc62", mDesc62));
                cmd.Parameters.Add(new SqlParameter("@Desc63", mDesc63));
                cmd.Parameters.Add(new SqlParameter("@Desc64", mDesc64));
                cmd.Parameters.Add(new SqlParameter("@Desc65", mDesc65));
                cmd.Parameters.Add(new SqlParameter("@Desc66", mDesc66));
                cmd.Parameters.Add(new SqlParameter("@Desc67", mDesc67));
                cmd.Parameters.Add(new SqlParameter("@Desc68", mDesc68));
                cmd.Parameters.Add(new SqlParameter("@Desc69", mDesc69));
                cmd.Parameters.Add(new SqlParameter("@Desc70", mDesc70));
                cmd.Parameters.Add(new SqlParameter("@Desc71", mDesc71));
                cmd.Parameters.Add(new SqlParameter("@Desc72", mDesc72));
                cmd.Parameters.Add(new SqlParameter("@Desc73", mDesc73));
                cmd.Parameters.Add(new SqlParameter("@Desc74", mDesc74));
                cmd.Parameters.Add(new SqlParameter("@Desc75", mDesc75));
                cmd.Parameters.Add(new SqlParameter("@Desc76", mDesc76));
                cmd.Parameters.Add(new SqlParameter("@Desc77", mDesc77));
                cmd.Parameters.Add(new SqlParameter("@Desc78", mDesc78));
                cmd.Parameters.Add(new SqlParameter("@Desc79", mDesc79));
                cmd.Parameters.Add(new SqlParameter("@Desc80", mDesc80));
                cmd.Parameters.Add(new SqlParameter("@Desc81", mDesc81));
                cmd.Parameters.Add(new SqlParameter("@Desc82", mDesc82));
                cmd.Parameters.Add(new SqlParameter("@Desc83", mDesc83));
                cmd.Parameters.Add(new SqlParameter("@Desc84", mDesc84));
                cmd.Parameters.Add(new SqlParameter("@Desc85", mDesc85));
                cmd.Parameters.Add(new SqlParameter("@Desc86", mDesc86));
                cmd.Parameters.Add(new SqlParameter("@Desc87", mDesc87));
                cmd.Parameters.Add(new SqlParameter("@Desc88", mDesc88));
                cmd.Parameters.Add(new SqlParameter("@Desc89", mDesc89));
                cmd.Parameters.Add(new SqlParameter("@Desc90", mDesc90));
                cmd.Parameters.Add(new SqlParameter("@Desc91", mDesc91));
                cmd.Parameters.Add(new SqlParameter("@Desc92", mDesc92));
                cmd.Parameters.Add(new SqlParameter("@Desc93", mDesc93));
                cmd.Parameters.Add(new SqlParameter("@Desc94", mDesc94));
                cmd.Parameters.Add(new SqlParameter("@Desc95", mDesc95));
                cmd.Parameters.Add(new SqlParameter("@Desc96", mDesc96));
                cmd.Parameters.Add(new SqlParameter("@Desc97", mDesc97));
                cmd.Parameters.Add(new SqlParameter("@Desc98", mDesc98));
                cmd.Parameters.Add(new SqlParameter("@Desc99", mDesc99));
                cmd.Parameters.Add(new SqlParameter("@Desc100", mDesc100));
                cmd.Parameters.Add(new SqlParameter("@UserID", mUserID));
                if (_dataAccess == null)
                {
                    ErrorTrackingExtension.SetError(new NullReferenceException("_dataAccess is null."));
                    return false;
                }
                bool _result = _dataAccess.ExecuteCommand(cmd);

                return _result;
            }
            catch (Exception exp)
            {
                ErrorTrackingExtension.SetError(exp);
                return false;
            }
        }


        public bool UpdateXmlTransInfo30(string comCode, string SQLprocName, string CallType, DataSet? parmXml01 = null, DataSet? parmXml02 = null,
  string mDesc1 = "", string mDesc2 = "", string mDesc3 = "", string mDesc4 = "", string mDesc5 = "", string mDesc6 = "",
             string mDesc7 = "", string mDesc8 = "", string mDesc9 = "", string mDesc10 = "", string mDesc11 = "", string mDesc12 = "",
             string mDesc13 = "", string mDesc14 = "", string mDesc15 = "", string mDesc16 = "", string mDesc17 = "",
             string mDesc18 = "", string mDesc19 = "", string mDesc20 = "", string mDesc21 = "", string mDesc22 = "",
             string mDesc23 = "", string mDesc24 = "",
          string mDesc25 = "", string mDesc26 = "", string mDesc27 = "", string mDesc28 = "", string mDesc29 = "", string mDesc30 = "")
        {
            try
            {
                ErrorTrackingExtension.ClearErrors();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SQLprocName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Comcod", comCode));
                cmd.Parameters.Add(new SqlParameter("@CallType", CallType));
                cmd.Parameters.Add("@Dxml01", SqlDbType.Xml).Value = parmXml01 == null ? null : parmXml01.GetXml();
                cmd.Parameters.Add("@Dxml02", SqlDbType.Xml).Value = parmXml02 == null ? null : parmXml02.GetXml();
                cmd.Parameters.Add(new SqlParameter("@Desc1", mDesc1));
                cmd.Parameters.Add(new SqlParameter("@Desc2", mDesc2));
                cmd.Parameters.Add(new SqlParameter("@Desc3", mDesc3));
                cmd.Parameters.Add(new SqlParameter("@Desc4", mDesc4));
                cmd.Parameters.Add(new SqlParameter("@Desc5", mDesc5));
                cmd.Parameters.Add(new SqlParameter("@Desc6", mDesc6));
                cmd.Parameters.Add(new SqlParameter("@Desc7", mDesc7));
                cmd.Parameters.Add(new SqlParameter("@Desc8", mDesc8));
                cmd.Parameters.Add(new SqlParameter("@Desc9", mDesc9));
                cmd.Parameters.Add(new SqlParameter("@Desc10", mDesc10));
                cmd.Parameters.Add(new SqlParameter("@Desc11", mDesc11));
                cmd.Parameters.Add(new SqlParameter("@Desc12", mDesc12));
                cmd.Parameters.Add(new SqlParameter("@Desc13", mDesc13));
                cmd.Parameters.Add(new SqlParameter("@Desc14", mDesc14));
                cmd.Parameters.Add(new SqlParameter("@Desc15", mDesc15));
                cmd.Parameters.Add(new SqlParameter("@Desc16", mDesc16));
                cmd.Parameters.Add(new SqlParameter("@Desc17", mDesc17));
                cmd.Parameters.Add(new SqlParameter("@Desc18", mDesc18));
                cmd.Parameters.Add(new SqlParameter("@Desc19", mDesc19));
                cmd.Parameters.Add(new SqlParameter("@Desc20", mDesc20));
                cmd.Parameters.Add(new SqlParameter("@Desc21", mDesc21));
                cmd.Parameters.Add(new SqlParameter("@Desc22", mDesc22));
                cmd.Parameters.Add(new SqlParameter("@Desc23", mDesc23));
                cmd.Parameters.Add(new SqlParameter("@Desc24", mDesc24));
                cmd.Parameters.Add(new SqlParameter("@Desc25", mDesc25));
                cmd.Parameters.Add(new SqlParameter("@Desc26", mDesc26));
                cmd.Parameters.Add(new SqlParameter("@Desc27", mDesc27));
                cmd.Parameters.Add(new SqlParameter("@Desc28", mDesc28));
                cmd.Parameters.Add(new SqlParameter("@Desc29", mDesc29));
                cmd.Parameters.Add(new SqlParameter("@Desc30", mDesc30));
                if (_dataAccess == null)
                {
                    ErrorTrackingExtension.SetError(new NullReferenceException("_dataAccess is null."));
                    return false;
                }
                bool _result = _dataAccess.ExecuteCommand(cmd);

                return _result;
            }
            catch (Exception exp)
            {
                ErrorTrackingExtension.SetError(exp);
                return false;
            }
        }

        public DataSet? GetTransInfoWithXML20(string comCode, string SQLprocName, string CallType, DataSet? parmXml01 = null, DataSet? parmXml02 = null, byte[]? parmBin01 = null,
  string mDesc1 = "", string mDesc2 = "", string mDesc3 = "", string mDesc4 = "", string mDesc5 = "", string mDesc6 = "", string mDesc7 = "", string mDesc8 = "",
  string mDesc9 = "", string mDesc10 = "", string mDesc11 = "", string mDesc12 = "", string mDesc13 = "", string mDesc14 = "", string mDesc15 = "",
  string mDesc16 = "", string mDesc17 = "", string mDesc18 = "", string mDesc19 = "", string mDesc20 = "", string @userid = "")
        {
            try
            {
                ErrorTrackingExtension.ClearErrors();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SQLprocName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Comcod", comCode));
                cmd.Parameters.Add(new SqlParameter("@CallType", CallType));
                cmd.Parameters.Add("@Dxml01", SqlDbType.Xml).Value = (parmXml01 == null ? null : parmXml01.GetXml());
                cmd.Parameters.Add("@Dxml02", SqlDbType.Xml).Value = (parmXml02 == null ? null : parmXml02.GetXml());
                cmd.Parameters.Add(new SqlParameter("@Dbin01", parmBin01));
                cmd.Parameters.Add(new SqlParameter("@Desc1", mDesc1));
                cmd.Parameters.Add(new SqlParameter("@Desc2", mDesc2));
                cmd.Parameters.Add(new SqlParameter("@Desc3", mDesc3));
                cmd.Parameters.Add(new SqlParameter("@Desc4", mDesc4));
                cmd.Parameters.Add(new SqlParameter("@Desc5", mDesc5));
                cmd.Parameters.Add(new SqlParameter("@Desc6", mDesc6));
                cmd.Parameters.Add(new SqlParameter("@Desc7", mDesc7));
                cmd.Parameters.Add(new SqlParameter("@Desc8", mDesc8));
                cmd.Parameters.Add(new SqlParameter("@Desc9", mDesc9));
                cmd.Parameters.Add(new SqlParameter("@Desc10", mDesc10));
                cmd.Parameters.Add(new SqlParameter("@Desc11", mDesc11));
                cmd.Parameters.Add(new SqlParameter("@Desc12", mDesc12));
                cmd.Parameters.Add(new SqlParameter("@Desc13", mDesc13));
                cmd.Parameters.Add(new SqlParameter("@Desc14", mDesc14));
                cmd.Parameters.Add(new SqlParameter("@Desc15", mDesc15));
                cmd.Parameters.Add(new SqlParameter("@Desc16", mDesc16));
                cmd.Parameters.Add(new SqlParameter("@Desc17", mDesc17));
                cmd.Parameters.Add(new SqlParameter("@Desc18", mDesc18));
                cmd.Parameters.Add(new SqlParameter("@Desc19", mDesc19));
                cmd.Parameters.Add(new SqlParameter("@Desc20", mDesc20));
                cmd.Parameters.Add(new SqlParameter("@UserID", @userid));

                if (_dataAccess == null)
                {
                    ErrorTrackingExtension.SetError(new NullReferenceException("_dataAccess is null."));
                    return null;
                }
                DataSet? result = _dataAccess.GetDataSet(cmd);

                return result;
            }
            catch (Exception exp)
            {
                ErrorTrackingExtension.SetError(exp);
                return null;
            }
        }

        // Add this method inside your existing class or as an extension
        public DataSet? GetTableDataFunction(
            string comCode,
            string functionName,           // e.g., "dbo.fn_GetTransInfo20"
            string callType,
            Dictionary<string, string>? descParams = null,
            string userid = "")
        {
            try
            {
                ErrorTrackingExtension.ClearErrors();

                // Build parameter list dynamically: @Desc1, @Desc2, ..., @Desc20
                var descParamNames = Enumerable.Range(1, 20)
                                                .Select(i => $"@Desc{i}")
                                                .ToList();

                var allParamNames = new List<string> { "@Comcod", "@CallType" };
                allParamNames.AddRange(descParamNames);
                if (!string.IsNullOrEmpty(userid))
                    allParamNames.Add("@UserID");

                // Build SQL: SELECT * FROM dbo.YourFunction(@Comcod, @CallType, @Desc1, ..., @Desc20, @UserID)
                string sql = $"SELECT * FROM {functionName}({string.Join(", ", allParamNames)})";

                using var cmd = new SqlCommand(sql);

                // Required parameters
                cmd.Parameters.Add("@Comcod", SqlDbType.NVarChar).Value = comCode;
                cmd.Parameters.Add("@CallType", SqlDbType.NVarChar).Value = callType;

                // Optional Desc1 to Desc20
                descParams ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                for (int i = 1; i <= 20; i++)
                {
                    string key = "Desc" + i; // Key in dictionary: Desc1, Desc2, ...
                    string paramName = "@Desc" + i;

                    if (descParams.TryGetValue(key, out string? value) && !string.IsNullOrWhiteSpace(value))
                        cmd.Parameters.Add(paramName, SqlDbType.NVarChar).Value = value;
                    else
                        cmd.Parameters.Add(paramName, SqlDbType.NVarChar).Value = DBNull.Value;
                    // Use DBNull.Value if your function has DEFAULT NULL
                    // Or use "" if it expects empty string
                }

                // UserID parameter
                cmd.Parameters.Add("@UserID", SqlDbType.NVarChar).Value =
                    string.IsNullOrEmpty(userid) ? (object)DBNull.Value : userid;

                // Reuse your existing GetDataSet method
                if (_dataAccess == null)
                {
                    ErrorTrackingExtension.SetError(new NullReferenceException("_dataAccess is null."));
                    return new DataSet();
                }
                return _dataAccess.GetDataSet(cmd);
            }
            catch (Exception exp)
            {
                ErrorTrackingExtension.SetError(exp);
                return null;
            }
        }
    }
}
