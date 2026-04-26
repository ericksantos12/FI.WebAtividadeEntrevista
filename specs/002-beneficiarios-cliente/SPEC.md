# Especificacao de Implementacao: Beneficiarios do Cliente

## Objetivo

Adicionar ao cadastro e alteracao de cliente um botao `Beneficiários` para abrir um pop-up de manutencao de beneficiarios, permitindo incluir, alterar e excluir registros com `CPF` e `Nome`, mantendo o padrao visual atual da tela e reutilizando as regras ja existentes de CPF.

## Escopo Funcional

- Exibir um novo botao `Beneficiários` no formulario compartilhado de cliente, seguindo o mesmo padrao visual dos demais botoes da tela e posicionado no lado oposto aos botoes `Salvar` e `Voltar`.
- O botao deve existir tanto em `Incluir` quanto em `Alterar`.
- Ao acionar o botao, abrir um pop-up/modal para manutencao dos beneficiarios do cliente.
- O pop-up deve conter:
  - campo `CPF`
  - campo `Nome`
  - botao principal com texto `Salvar`, tanto para inclusao quanto para edicao
  - grid com os beneficiarios ja adicionados na sessao da tela, exibindo as colunas `CPF` e `Nome`
- O grid deve permitir manutencao completa no proprio pop-up:
  - alterar um beneficiario existente, preenchendo novamente os campos `CPF` e `Nome`
  - excluir um beneficiario existente, com confirmacao antes da remocao
- Ao fechar e reabrir o pop-up na mesma tela, o rascunho em memoria deve ser preservado.
- O campo `CPF` do beneficiario deve seguir a mesma mascara visual do CPF do cliente no formato `999.999.999-99`.
- O campo `CPF` do beneficiario deve seguir as mesmas regras de negocio do CPF do cliente, incluindo normalizacao para somente numeros e validacao dos digitos verificadores.
- Nao deve ser possivel incluir dois beneficiarios com o mesmo CPF para o mesmo cliente.
- A mensagem de duplicidade no fluxo do pop-up deve ser `CPF já incluído`.
- O mesmo CPF de beneficiario pode existir para clientes diferentes.
- O cliente pode ser salvo sem nenhum beneficiario cadastrado.
- As operacoes no pop-up nao devem gravar imediatamente na base.
- Os beneficiarios devem ser persistidos somente quando o usuario clicar em `Salvar` no formulario principal do cliente.
- Na tela de alteracao, os beneficiarios ja existentes do cliente devem ser carregados no pop-up para manutencao.

## Arquitetura Afetada

- Web MVC:
  - `FI.WebAtividadeEntrevista/Views/Cliente/Forms.cshtml`
  - `FI.WebAtividadeEntrevista/Views/Cliente/Incluir.cshtml`
  - `FI.WebAtividadeEntrevista/Views/Cliente/Alterar.cshtml`
  - `FI.WebAtividadeEntrevista/Controllers/ClienteController.cs`
  - `FI.WebAtividadeEntrevista/Models/ClienteModel.cs`
  - `FI.WebAtividadeEntrevista/Models/Validacao/CpfValidationAttribute.cs`
  - `FI.WebAtividadeEntrevista/Scripts/Clientes/FI.Clientes.js`
  - `FI.WebAtividadeEntrevista/Scripts/Clientes/FI.AltClientes.js`
- Biblioteca de dominio/negocio/dados:
  - `FI.AtividadeEntrevista/Validacao/Cpf.cs`
  - novo DML para beneficiario
  - possivel evolucao do DML de cliente para transportar a colecao de beneficiarios
  - novas classes de BLL/DAL para beneficiarios, seguindo o padrao atual de cliente
- Banco de dados:
  - tabela `BENEFICIARIOS`
  - procedures para consulta, inclusao, alteracao, exclusao e verificacao de duplicidade por cliente
  - carregamento de beneficiarios na consulta de cliente para alteracao

## Regras de Validacao

- `CPF` do beneficiario e obrigatorio.
- `Nome` do beneficiario e obrigatorio.
- O CPF deve aceitar entrada mascarada e ser normalizado para conter apenas numeros antes da validacao e da persistencia.
- O CPF deve conter exatamente 11 digitos.
- Devem ser rejeitadas sequencias repetidas, como `00000000000`, `11111111111` e equivalentes.
- Devem ser validados os dois digitos verificadores com a mesma regra ja aplicada ao CPF do cliente.
- A validacao deve reutilizar a logica centralizada existente em `FI.AtividadeEntrevista/Validacao/Cpf.cs`.
- A duplicidade deve ser verificada com base no CPF normalizado.
- O sistema deve impedir duplicidade do mesmo CPF dentro da colecao em memoria do cliente antes do submit.
- A mensagem apresentada para duplicidade no contexto do mesmo cliente deve ser `CPF já incluído`.
- O backend deve validar novamente duplicidade para o mesmo cliente antes da persistencia.
- O banco deve reforcar a regra com restricao unica por `IDCLIENTE + CPF`.
- As mensagens de erro devem seguir o padrao atual da tela, sendo retornadas pelo backend e exibidas no frontend em modal.

## Decisao Tecnica

Os beneficiarios serao mantidos no frontend como uma colecao em memoria durante a edicao da tela de cliente. Essa colecao deve ser carregada vazia na inclusao e pre-preenchida na alteracao.

Ao salvar o cliente, a colecao completa de beneficiarios deve ser serializada em um campo oculto do formulario principal e enviada junto com os dados do cliente. O backend deve receber a lista completa e tratar a persistencia como um snapshot final da tela.

Na inclusao, o cliente deve ser salvo primeiro para gerar o `IDCLIENTE`, e somente depois os beneficiarios devem ser persistidos.

Na alteracao, o backend deve comparar os beneficiarios recebidos com os registros atualmente gravados para o cliente, aplicando inclusoes, alteracoes e exclusoes conforme necessario.

Ao excluir um cliente, os beneficiarios vinculados tambem devem ser excluidos na mesma operacao.

O pop-up de beneficiarios deve ser um modal dedicado na view, com estrutura apropriada para campos e grid. Nao e recomendado reutilizar o `ModalDialog(...)` atual de `FI.Clientes.js` e `FI.AltClientes.js`, pois esse componente hoje atende apenas mensagens simples de sucesso e erro.

O grid do modal deve ser client-side e refletir imediatamente o estado atual do rascunho em memoria, sem persistencia imediata.

O botao principal do modal permanece com o texto `Salvar` tanto na inclusao quanto na edicao de um beneficiario.

A exclusao de um beneficiario no modal deve solicitar confirmacao antes de remover o item do rascunho.

Ao fechar o modal e reabri-lo na mesma tela, o estado do rascunho deve ser mantido.

A regra de CPF deve ser reaproveitada do comportamento atual do cliente, incluindo:
- mascara visual no frontend
- normalizacao para somente numeros
- validacao algoritmica dos digitos verificadores
- comparacao por CPF normalizado para duplicidade

## Banco de Dados

Criar a tabela `BENEFICIARIOS` com as colunas:
- `ID`
- `CPF`
- `NOME`
- `IDCLIENTE`

O campo `CPF` deve ser armazenado como `VARCHAR(11)`, contendo apenas numeros.

O relacionamento com `CLIENTES` deve ocorrer por `IDCLIENTE`.

Criar um indice unico composto para garantir a unicidade do CPF por cliente, por exemplo:

```sql
CREATE UNIQUE INDEX UX_BENEFICIARIOS_IDCLIENTE_CPF
ON BENEFICIARIOS (IDCLIENTE, CPF);
```

Criar ou versionar procedures seguindo o padrao atual do projeto para:
- consultar beneficiarios por cliente
- incluir beneficiario
- alterar beneficiario
- excluir beneficiario
- verificar existencia de beneficiario com mesmo `CPF` para o mesmo `IDCLIENTE`
- excluir beneficiarios vinculados quando um cliente for removido

A especificacao deve considerar tambem o carregamento dos beneficiarios no fluxo de alteracao do cliente, seja por ajuste da consulta atual do cliente ou por consulta dedicada complementar.

## Criterios de Aceite

- O formulario de cliente exibe o botao `Beneficiários` nas telas de inclusao e alteracao, posicionado no lado oposto aos botoes `Salvar` e `Voltar`.
- O botao abre um pop-up com campos `CPF` e `Nome`, botao `Salvar` e um grid com as colunas `CPF` e `Nome`.
- O CPF do beneficiario usa a mesma mascara e a mesma regra de validacao do CPF do cliente.
- O pop-up permite incluir, alterar e excluir beneficiarios reutilizando os mesmos campos para edicao.
- A exclusao de um beneficiario no pop-up exige confirmacao.
- Ao fechar e reabrir o pop-up na mesma tela, o rascunho de beneficiarios e mantido.
- O sistema impede incluir CPF duplicado para o mesmo cliente e exibe a mensagem `CPF já incluído`.
- O sistema permite o mesmo CPF vinculado a clientes diferentes.
- O cliente pode ser salvo sem beneficiarios.
- Na inclusao de cliente, os beneficiarios so sao gravados quando o usuario clicar em `Salvar`.
- Na alteracao de cliente, inclusoes, edicoes e exclusoes de beneficiarios tambem so sao gravadas quando o usuario clicar em `Salvar`.
- Ao abrir a alteracao de um cliente existente, os beneficiarios ja cadastrados sao carregados para manutencao.
- Ao excluir um cliente, os beneficiarios vinculados tambem sao excluidos.
- Os dados sao persistidos na tabela `BENEFICIARIOS` com as colunas `ID`, `CPF`, `NOME` e `IDCLIENTE`.
- O padrao visual do novo botao, dos campos e do modal permanece consistente com a tela atual de cliente.
