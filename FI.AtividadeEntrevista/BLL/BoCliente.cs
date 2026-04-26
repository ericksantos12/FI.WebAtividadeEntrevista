using System.Collections.Generic;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoCliente
    {
        public long Incluir(DML.Cliente cliente)
        {
            PrepararCliente(cliente);

            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.Incluir(cliente);
        }

        public void Alterar(DML.Cliente cliente)
        {
            PrepararCliente(cliente);

            DAL.DaoCliente cli = new DAL.DaoCliente();
            cli.Alterar(cliente);
        }

        public DML.Cliente Consultar(long id)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.Consultar(id);
        }

        public void Excluir(long id)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            cli.Excluir(id);
        }

        public List<DML.Cliente> Listar()
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.Listar();
        }

        public List<DML.Cliente> Pesquisa(int iniciarEm, int quantidade, string campoOrdenacao, bool crescente, out int qtd)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.Pesquisa(iniciarEm, quantidade, campoOrdenacao, crescente, out qtd);
        }

        public bool VerificarExistencia(string CPF)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.VerificarExistencia(Validacao.Cpf.Normalizar(CPF));
        }

        public bool VerificarExistencia(string CPF, long id)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.VerificarExistencia(Validacao.Cpf.Normalizar(CPF), id);
        }

        private void PrepararCliente(DML.Cliente cliente)
        {
            cliente.CPF = Validacao.Cpf.Normalizar(cliente.CPF);
            cliente.Beneficiarios = cliente.Beneficiarios ?? new List<DML.Beneficiario>();

            foreach (DML.Beneficiario beneficiario in cliente.Beneficiarios)
            {
                beneficiario.CPF = Validacao.Cpf.Normalizar(beneficiario.CPF);
                beneficiario.Nome = beneficiario.Nome?.Trim();
            }
        }
    }
}
