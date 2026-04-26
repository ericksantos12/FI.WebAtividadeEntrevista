using FI.AtividadeEntrevista.Validacao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebAtividadeEntrevista.Models.Validacao;

namespace WebAtividadeEntrevista.Models
{
    /// <summary>
    /// Classe de Modelo de Cliente
    /// </summary>
    public class ClienteModel : IValidatableObject
    {
        public long Id { get; set; }

        public ClienteModel()
        {
            Beneficiarios = new List<BeneficiarioModel>();
        }

        [JsonIgnore]
        public List<BeneficiarioModel> Beneficiarios { get; set; }

        [JsonIgnore]
        public bool BeneficiariosJsonValido { get; private set; }

        public string BeneficiariosJson { get; set; }

        public void CarregarBeneficiarios()
        {
            BeneficiariosJsonValido = true;

            if (string.IsNullOrWhiteSpace(BeneficiariosJson))
            {
                Beneficiarios = new List<BeneficiarioModel>();
                return;
            }

            try
            {
                Beneficiarios = JsonConvert.DeserializeObject<List<BeneficiarioModel>>(BeneficiariosJson) ?? new List<BeneficiarioModel>();
            }
            catch (JsonException)
            {
                BeneficiariosJsonValido = false;
                Beneficiarios = new List<BeneficiarioModel>();
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            CarregarBeneficiarios();

            if (!BeneficiariosJsonValido)
            {
                yield return new ValidationResult("Os beneficiarios informados sao invalidos");
                yield break;
            }

            for (int i = 0; i < Beneficiarios.Count; i++)
            {
                BeneficiarioModel beneficiario = Beneficiarios[i];
                string cpf = beneficiario.CPF;
                string nome = beneficiario.Nome;

                if (string.IsNullOrWhiteSpace(cpf))
                    yield return new ValidationResult("O CPF do beneficiario e obrigatorio");
                else if (!Cpf.Validar(cpf))
                    yield return new ValidationResult("Digite um CPF valido para o beneficiario");

                if (string.IsNullOrWhiteSpace(nome))
                    yield return new ValidationResult("O nome do beneficiario e obrigatorio");
            }
        }

        /// <summary>
        /// CPF
        /// </summary>
        [Required(ErrorMessage = "O CPF e obrigatorio")]
        [CpfValidation(ErrorMessage = "Digite um CPF valido")]
        public string CPF { get; set; }

        /// <summary>
        /// CEP
        /// </summary>
        [Required]
        public string CEP { get; set; }

        /// <summary>
        /// Cidade
        /// </summary>
        [Required]
        public string Cidade { get; set; }

        /// <summary>
        /// E-mail
        /// </summary>
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Digite um e-mail válido")]
        public string Email { get; set; }

        /// <summary>
        /// Estado
        /// </summary>
        [Required]
        [MaxLength(2)]
        public string Estado { get; set; }

        /// <summary>
        /// Logradouro
        /// </summary>
        [Required]
        public string Logradouro { get; set; }

        /// <summary>
        /// Nacionalidade
        /// </summary>
        [Required]
        public string Nacionalidade { get; set; }

        /// <summary>
        /// Nome
        /// </summary>
        [Required]
        public string Nome { get; set; }

        /// <summary>
        /// Sobrenome
        /// </summary>
        [Required]
        public string Sobrenome { get; set; }

        /// <summary>
        /// Telefone
        /// </summary>
        public string Telefone { get; set; }
    }
}
