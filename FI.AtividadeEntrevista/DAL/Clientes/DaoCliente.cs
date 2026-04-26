using FI.AtividadeEntrevista.DAL.Beneficiarios;
using FI.AtividadeEntrevista.DML;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FI.AtividadeEntrevista.DAL
{
    internal class DaoCliente : AcessoDados
    {
        internal long Incluir(DML.Cliente cliente)
        {
            using (SqlConnection conexao = CriarConexao())
            {
                conexao.Open();
                SqlTransaction transacao = conexao.BeginTransaction();

                try
                {
                    long idCliente = IncluirCliente(cliente, conexao, transacao);
                    cliente.Id = idCliente;
                    PersistirBeneficiarios(cliente, conexao, transacao);
                    transacao.Commit();
                    return idCliente;
                }
                catch
                {
                    transacao.Rollback();
                    throw;
                }
            }
        }

        internal DML.Cliente Consultar(long Id)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("Id", Id));

            DataSet ds = base.Consultar("FI_SP_ConsCliente", parametros);
            List<DML.Cliente> cli = Converter(ds);
            DML.Cliente cliente = cli.FirstOrDefault();

            if (cliente != null)
                cliente.Beneficiarios = new DaoBeneficiario().Listar(cliente.Id);

            return cliente;
        }

        internal bool VerificarExistencia(string CPF)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("CPF", CPF));

            DataSet ds = base.Consultar("FI_SP_VerificaCliente", parametros);
            return ds.Tables[0].Rows.Count > 0;
        }

        internal bool VerificarExistencia(string CPF, long id)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("CPF", CPF));
            parametros.Add(new SqlParameter("Id", id));

            DataSet ds = base.Consultar("FI_SP_VerificaCliente", parametros);
            return ds.Tables[0].Rows.Count > 0;
        }

        internal List<Cliente> Pesquisa(int iniciarEm, int quantidade, string campoOrdenacao, bool crescente, out int qtd)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("iniciarEm", iniciarEm));
            parametros.Add(new SqlParameter("quantidade", quantidade));
            parametros.Add(new SqlParameter("campoOrdenacao", campoOrdenacao));
            parametros.Add(new SqlParameter("crescente", crescente));

            DataSet ds = base.Consultar("FI_SP_PesqCliente", parametros);
            List<DML.Cliente> cli = Converter(ds);

            int iQtd = 0;
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                int.TryParse(ds.Tables[1].Rows[0][0].ToString(), out iQtd);

            qtd = iQtd;
            return cli;
        }

        internal List<DML.Cliente> Listar()
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("Id", 0));

            DataSet ds = base.Consultar("FI_SP_ConsCliente", parametros);
            return Converter(ds);
        }

        internal void Alterar(DML.Cliente cliente)
        {
            using (SqlConnection conexao = CriarConexao())
            {
                conexao.Open();
                SqlTransaction transacao = conexao.BeginTransaction();

                try
                {
                    AlterarCliente(cliente, conexao, transacao);
                    PersistirBeneficiarios(cliente, conexao, transacao);
                    transacao.Commit();
                }
                catch
                {
                    transacao.Rollback();
                    throw;
                }
            }
        }

        internal void Excluir(long Id)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("Id", Id));

            base.Executar("FI_SP_DelCliente", parametros);
        }

        private long IncluirCliente(DML.Cliente cliente, SqlConnection conexao, SqlTransaction transacao)
        {
            List<SqlParameter> parametros = ObterParametrosCliente(cliente);
            DataSet ds = base.Consultar("FI_SP_IncClienteV2", parametros, conexao, transacao);
            long ret = 0;
            if (ds.Tables[0].Rows.Count > 0)
                long.TryParse(ds.Tables[0].Rows[0][0].ToString(), out ret);
            return ret;
        }

        private void AlterarCliente(DML.Cliente cliente, SqlConnection conexao, SqlTransaction transacao)
        {
            List<SqlParameter> parametros = ObterParametrosCliente(cliente);
            parametros.Add(new SqlParameter("ID", cliente.Id));
            base.Executar("FI_SP_AltCliente", parametros, conexao, transacao);
        }

        private List<SqlParameter> ObterParametrosCliente(DML.Cliente cliente)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter("Nome", cliente.Nome));
            parametros.Add(new SqlParameter("Sobrenome", cliente.Sobrenome));
            parametros.Add(new SqlParameter("Nacionalidade", cliente.Nacionalidade));
            parametros.Add(new SqlParameter("CEP", cliente.CEP));
            parametros.Add(new SqlParameter("Estado", cliente.Estado));
            parametros.Add(new SqlParameter("Cidade", cliente.Cidade));
            parametros.Add(new SqlParameter("Logradouro", cliente.Logradouro));
            parametros.Add(new SqlParameter("Email", cliente.Email));
            parametros.Add(new SqlParameter("Telefone", cliente.Telefone));
            parametros.Add(new SqlParameter("CPF", cliente.CPF));
            return parametros;
        }

        private void PersistirBeneficiarios(Cliente cliente, SqlConnection conexao, SqlTransaction transacao)
        {
            DaoBeneficiario daoBeneficiario = new DaoBeneficiario();
            List<Beneficiario> atuais = daoBeneficiario.Listar(cliente.Id, conexao, transacao);
            List<Beneficiario> informados = cliente.Beneficiarios ?? new List<Beneficiario>();

            foreach (Beneficiario beneficiario in informados)
            {
                beneficiario.IdCliente = cliente.Id;

                if (beneficiario.Id == 0)
                {
                    daoBeneficiario.Incluir(beneficiario, conexao, transacao);
                    continue;
                }

                if (!atuais.Any(x => x.Id == beneficiario.Id))
                    throw new DataException("Beneficiario invalido para o cliente informado");

                daoBeneficiario.Alterar(beneficiario, conexao, transacao);
            }

            foreach (Beneficiario beneficiarioAtual in atuais)
            {
                if (!informados.Any(x => x.Id == beneficiarioAtual.Id))
                    daoBeneficiario.Excluir(beneficiarioAtual.Id, conexao, transacao);
            }
        }

        private List<DML.Cliente> Converter(DataSet ds)
        {
            List<DML.Cliente> lista = new List<DML.Cliente>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DML.Cliente cli = new DML.Cliente();
                    cli.Id = row.Field<long>("Id");
                    cli.CPF = row.Field<string>("CPF");
                    cli.CEP = row.Field<string>("CEP");
                    cli.Cidade = row.Field<string>("Cidade");
                    cli.Email = row.Field<string>("Email");
                    cli.Estado = row.Field<string>("Estado");
                    cli.Logradouro = row.Field<string>("Logradouro");
                    cli.Nacionalidade = row.Field<string>("Nacionalidade");
                    cli.Nome = row.Field<string>("Nome");
                    cli.Sobrenome = row.Field<string>("Sobrenome");
                    cli.Telefone = row.Field<string>("Telefone");
                    lista.Add(cli);
                }
            }

            return lista;
        }
    }
}
