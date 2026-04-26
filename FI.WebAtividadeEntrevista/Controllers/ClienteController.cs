using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using FI.AtividadeEntrevista.Validacao;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }

            model.CarregarBeneficiarios();
            model.CPF = Cpf.Normalizar(model.CPF);

            if (bo.VerificarExistencia(model.CPF))
            {
                Response.StatusCode = 400;
                return Json("CPF ja cadastrado");
            }

            string erroBeneficiarios = ValidarBeneficiarios(model.Beneficiarios);
            if (!string.IsNullOrWhiteSpace(erroBeneficiarios))
            {
                Response.StatusCode = 400;
                return Json(erroBeneficiarios);
            }

            try
            {
                model.Id = bo.Incluir(MapearCliente(model));
                return Json("Cadastro efetuado com sucesso");
            }
            catch (DataException ex)
            {
                Response.StatusCode = 400;
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }

            model.CarregarBeneficiarios();
            model.CPF = Cpf.Normalizar(model.CPF);

            if (bo.VerificarExistencia(model.CPF, model.Id))
            {
                Response.StatusCode = 400;
                return Json("CPF ja cadastrado");
            }

            string erroBeneficiarios = ValidarBeneficiarios(model.Beneficiarios);
            if (!string.IsNullOrWhiteSpace(erroBeneficiarios))
            {
                Response.StatusCode = 400;
                return Json(erroBeneficiarios);
            }

            try
            {
                bo.Alterar(MapearCliente(model));
                return Json("Cadastro alterado com sucesso");
            }
            catch (DataException ex)
            {
                Response.StatusCode = 400;
                return Json(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CPF = cliente.CPF,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    Beneficiarios = cliente.Beneficiarios == null
                        ? new List<BeneficiarioModel>()
                        : cliente.Beneficiarios.Select(x => new BeneficiarioModel()
                        {
                            Id = x.Id,
                            CPF = x.CPF,
                            Nome = x.Nome
                        }).ToList()
                };
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        private Cliente MapearCliente(ClienteModel model)
        {
            return new Cliente()
            {
                Id = model.Id,
                CPF = model.CPF,
                CEP = model.CEP,
                Cidade = model.Cidade,
                Email = model.Email,
                Estado = model.Estado,
                Logradouro = model.Logradouro,
                Nacionalidade = model.Nacionalidade,
                Nome = model.Nome,
                Sobrenome = model.Sobrenome,
                Telefone = model.Telefone,
                Beneficiarios = model.Beneficiarios.Select(x => new Beneficiario()
                {
                    Id = x.Id,
                    CPF = x.CPF,
                    Nome = x.Nome
                }).ToList()
            };
        }

        private string ValidarBeneficiarios(List<BeneficiarioModel> beneficiarios)
        {
            List<BeneficiarioModel> lista = beneficiarios ?? new List<BeneficiarioModel>();
            HashSet<string> cpfs = new HashSet<string>();

            foreach (BeneficiarioModel beneficiario in lista)
            {
                string cpfNormalizado = Cpf.Normalizar(beneficiario.CPF);

                if (cpfs.Contains(cpfNormalizado))
                    return "CPF já incluído";

                cpfs.Add(cpfNormalizado);
            }

            return null;
        }
    }
}
