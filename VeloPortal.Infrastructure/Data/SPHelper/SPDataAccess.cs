using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloPortal.Domain.Extensions;

namespace VeloPortal.Infrastructure.Data.SPHelper
{
    public class SPDataAccess
    {
        //   public readonly IConfiguration _configuration;

        public SqlConnection m_Conn;
        private Hashtable m_Erroobj;
        public SPDataAccess(string connectionsring)
        {
            m_Conn = new SqlConnection(connectionsring);
            m_Erroobj = new Hashtable();
        }

        public DataSet? GetDataSet(SqlCommand Cmd)
        {
            try
            {
                SqlDataAdapter adp = new SqlDataAdapter();
                adp.SelectCommand = Cmd;
                Cmd.CommandTimeout = 0;
                Cmd.Connection = m_Conn;

                DataSet ds = new DataSet();
                adp.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return null;
            }
        }

        public bool ExecuteCommand(SqlCommand cmd)
        {
            cmd.Connection = m_Conn;
            cmd.CommandTimeout = 120;
            try
            {
                m_Conn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return false;
            }
            finally
            {
                m_Conn.Close();
            }
        }
    }
}
