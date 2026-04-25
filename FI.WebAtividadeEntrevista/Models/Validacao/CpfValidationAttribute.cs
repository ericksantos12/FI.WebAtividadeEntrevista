using System.ComponentModel.DataAnnotations;
using FI.AtividadeEntrevista.Validacao;

namespace WebAtividadeEntrevista.Models.Validacao
{
    public class CpfValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string cpf = value as string;

            if (string.IsNullOrWhiteSpace(cpf))
                return true;

            return Cpf.Validar(cpf);
        }
    }
}
