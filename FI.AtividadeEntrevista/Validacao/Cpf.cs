using System.Linq;

namespace FI.AtividadeEntrevista.Validacao
{
    public static class Cpf
    {
        public static string Normalizar(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return string.Empty;

            return new string(cpf.Where(char.IsDigit).ToArray());
        }

        public static bool Validar(string cpf)
        {
            string normalizado = Normalizar(cpf);

            if (normalizado.Length != 11)
                return false;

            if (normalizado.Distinct().Count() == 1)
                return false;

            int primeiroDigito = CalcularDigito(normalizado, 9, 10);
            int segundoDigito = CalcularDigito(normalizado, 10, 11);

            return normalizado[9] == primeiroDigito.ToString()[0]
                && normalizado[10] == segundoDigito.ToString()[0];
        }

        private static int CalcularDigito(string cpf, int quantidadeDigitos, int pesoInicial)
        {
            int soma = 0;

            for (int i = 0; i < quantidadeDigitos; i++)
                soma += (cpf[i] - '0') * (pesoInicial - i);

            int resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }
    }
}
