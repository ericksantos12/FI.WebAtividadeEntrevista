using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FI.AtividadeEntrevista.DAL
{
    internal class AcessoDados
    {
        private string stringDeConexao
        {
            get
            {
                ConnectionStringSettings conn = ConfigurationManager.ConnectionStrings["BancoDeDados"];
                if (conn != null)
                    return conn.ConnectionString;
                else
                    return string.Empty;
            }
        }

        internal SqlConnection CriarConexao()
        {
            return new SqlConnection(stringDeConexao);
        }

        internal void Executar(string NomeProcedure, List<SqlParameter> parametros)
        {
            using (SqlConnection conexao = CriarConexao())
            {
                conexao.Open();
                Executar(NomeProcedure, parametros, conexao, null);
            }
        }

        internal void Executar(string NomeProcedure, List<SqlParameter> parametros, SqlConnection conexao, SqlTransaction transacao)
        {
            SqlCommand comando = new SqlCommand();
            comando.Connection = conexao;
            comando.Transaction = transacao;
            comando.CommandType = CommandType.StoredProcedure;
            comando.CommandText = NomeProcedure;
            foreach (var item in parametros)
                comando.Parameters.Add(item);

            comando.ExecuteNonQuery();
        }

        internal DataSet Consultar(string NomeProcedure, List<SqlParameter> parametros)
        {
            using (SqlConnection conexao = CriarConexao())
            {
                conexao.Open();
                return Consultar(NomeProcedure, parametros, conexao, null);
            }
        }

        internal DataSet Consultar(string NomeProcedure, List<SqlParameter> parametros, SqlConnection conexao, SqlTransaction transacao)
        {
            SqlCommand comando = new SqlCommand();
            comando.Connection = conexao;
            comando.Transaction = transacao;
            comando.CommandType = CommandType.StoredProcedure;
            comando.CommandText = NomeProcedure;
            foreach (var item in parametros)
                comando.Parameters.Add(item);

            SqlDataAdapter adapter = new SqlDataAdapter(comando);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds;
        }
    }
}
