using FI.AtividadeEntrevista.DML;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FI.AtividadeEntrevista.DAL.Beneficiarios
{
    internal class DaoBeneficiario : AcessoDados
    {
        internal List<Beneficiario> Listar(long idCliente)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("IDCLIENTE", idCliente));

            DataSet ds = base.Consultar("FI_SP_ConsBeneficiarios", parametros);
            return Converter(ds);
        }

        internal List<Beneficiario> Listar(long idCliente, SqlConnection conexao, SqlTransaction transacao)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("IDCLIENTE", idCliente));

            DataSet ds = base.Consultar("FI_SP_ConsBeneficiarios", parametros, conexao, transacao);
            return Converter(ds);
        }

        internal void Incluir(Beneficiario beneficiario, SqlConnection conexao, SqlTransaction transacao)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("CPF", beneficiario.CPF));
            parametros.Add(new SqlParameter("NOME", beneficiario.Nome));
            parametros.Add(new SqlParameter("IDCLIENTE", beneficiario.IdCliente));

            DataSet ds = base.Consultar("FI_SP_IncBeneficiario", parametros, conexao, transacao);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                long id = 0;
                long.TryParse(ds.Tables[0].Rows[0][0].ToString(), out id);
                beneficiario.Id = id;
            }
        }

        internal void Alterar(Beneficiario beneficiario, SqlConnection conexao, SqlTransaction transacao)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("ID", beneficiario.Id));
            parametros.Add(new SqlParameter("CPF", beneficiario.CPF));
            parametros.Add(new SqlParameter("NOME", beneficiario.Nome));

            base.Executar("FI_SP_AltBeneficiario", parametros, conexao, transacao);
        }


        internal void Excluir(long id, SqlConnection conexao, SqlTransaction transacao)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("ID", id));

            base.Executar("FI_SP_DelBeneficiario", parametros, conexao, transacao);
        }

        private List<Beneficiario> Converter(DataSet ds)
        {
            List<Beneficiario> lista = new List<Beneficiario>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Beneficiario beneficiario = new Beneficiario();
                    beneficiario.Id = row.Field<long>("ID");
                    beneficiario.CPF = row.Field<string>("CPF");
                    beneficiario.Nome = row.Field<string>("NOME");
                    beneficiario.IdCliente = row.Field<long>("IDCLIENTE");
                    lista.Add(beneficiario);
                }
            }

            return lista;
        }
    }
}
