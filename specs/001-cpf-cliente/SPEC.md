# Especificacao de Implementacao: Campo CPF do Cliente

## Objetivo

Adicionar o campo CPF ao cadastro e alteracao de cliente, mantendo o padrao visual atual da tela, validando obrigatoriedade, mascara, digitos verificadores e impedindo duplicidade na base.

## Escopo Funcional

- Exibir o campo `CPF` abaixo do campo `Nome` no formulario de cliente.
- O input deve ocupar a mesma largura do campo `Nome`, encaixado no grid Bootstrap atual.
- O campo deve seguir o padrao visual dos demais campos: `form-group`, `label`, `form-control`, placeholder e validacao visual HTML.
- O CPF deve ser obrigatorio.
- O CPF deve aceitar mascara no formato `999.999.999-99`.
- O CPF deve ser validado pelo calculo padrao dos dois digitos verificadores.
- O sistema nao deve permitir cadastrar CPF ja existente.
- Na alteracao, o CPF do cliente deve ser carregado e persistido. Se a regra permitir alteracao do CPF, a verificacao de duplicidade deve ignorar o proprio registro.

## Arquitetura Afetada

- Web MVC:
  - `FI.WebAtividadeEntrevista/Models/ClienteModel.cs`
  - `FI.WebAtividadeEntrevista/Controllers/ClienteController.cs`
  - `FI.WebAtividadeEntrevista/Views/Cliente/Forms.cshtml`
  - `FI.WebAtividadeEntrevista/Scripts/Clientes/FI.Clientes.js`
  - `FI.WebAtividadeEntrevista/Scripts/Clientes/FI.AltClientes.js`
- Biblioteca de dominio/negocio/dados:
  - `FI.AtividadeEntrevista/DML/Cliente.cs`
  - `FI.AtividadeEntrevista/BLL/BoCliente.cs`
  - `FI.AtividadeEntrevista/DAL/Clientes/DaoCliente.cs`
- Banco de dados:
  - Tabela `CLIENTES`
  - Procedures `FI_SP_IncClienteV2`, `FI_SP_AltCliente`, `FI_SP_ConsCliente`, `FI_SP_PesqCliente`
  - Criar ou versionar `FI_SP_VerificaCliente`, pois o codigo ja chama essa procedure, mas ela nao existe na pasta `Procedures`.

## Regras de Validacao

- Remover caracteres nao numericos antes do calculo.
- CPF deve conter exatamente 11 digitos.
- Rejeitar sequencias repetidas, como `00000000000`, `11111111111` etc.
- Calcular o primeiro digito com pesos de 10 a 2.
- Calcular o segundo digito com pesos de 11 a 2.
- CPF mascarado deve ser preservado ou normalizado conforme decisao tecnica, mas a validacao e a verificacao de duplicidade devem usar formato consistente.
- Mensagens de erro devem seguir o padrao atual retornado pelo `ModelState` e exibido em modal no frontend.

## Decisao Tecnica

O CPF sera salvo somente com numeros, sem mascara, usando 11 digitos (`VARCHAR(11)`). A mascara `999.999.999-99` deve existir apenas na interface. Validacao de digitos verificadores e verificacao de duplicidade devem usar sempre o valor normalizado.

## Banco de Dados

Adicionar coluna `CPF` na tabela `CLIENTES` como `VARCHAR(11)`, persistindo apenas numeros.

Criar indice unico para CPF, garantindo a regra tambem no banco:

```sql
CREATE UNIQUE INDEX UX_CLIENTES_CPF ON CLIENTES (CPF);
```

Atualizar as procedures para incluir CPF nos parametros, inserts, updates e selects.

## Criterios de Aceite

- Campo CPF aparece abaixo de Nome e alinhado ao grid atual.
- Campo CPF e obrigatorio no cadastro e na alteracao.
- CPF invalido nao e aceito.
- CPF duplicado nao e cadastrado.
- CPF valido e unico e salvo corretamente.
- Cliente existente carrega CPF na tela de alteracao.
- Listagem e demais campos existentes continuam funcionando.
