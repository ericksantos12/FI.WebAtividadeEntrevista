# Tasks: Beneficiarios do Cliente

## Fase 1: Decisoes e Preparacao

- [x] Confirmar o nome final da pasta da especificacao como `002-beneficiarios-cliente`.
- [x] Consolidar na especificacao a decisao de persistencia adiada por snapshot completo dos beneficiarios.
- [x] Consolidar na especificacao o uso de modal dedicado para manutencao de beneficiarios.
- [x] Registrar na especificacao o reuso obrigatorio da validacao e normalizacao de CPF ja existentes.
- [x] Definir o posicionamento do botao `Beneficiários` no lado oposto aos botoes `Salvar` e `Voltar`.
- [x] Definir que o cliente pode ser salvo sem beneficiarios.
- [x] Definir que, ao excluir cliente, os beneficiarios vinculados tambem devem ser excluidos.
- [x] Definir que o rascunho de beneficiarios permanece ao fechar e reabrir o pop-up na mesma tela.
- [x] Definir que o grid do pop-up exibira as colunas `CPF` e `Nome`.
- [x] Definir a mensagem de duplicidade como `CPF já incluído`.
- [x] Definir que a exclusao de beneficiario no pop-up exige confirmacao.
- [x] Definir que o botao principal do pop-up permanece com o texto `Salvar` durante a edicao.

## Fase 2: Banco de Dados

- [x] Criar a tabela `BENEFICIARIOS` com os campos `ID`, `CPF`, `NOME` e `IDCLIENTE`.
- [x] Criar indice unico composto para `IDCLIENTE + CPF`.
- [x] Criar ou versionar procedure para consultar beneficiarios por cliente.
- [x] Criar ou versionar procedure para incluir beneficiario.
- [x] Criar ou versionar procedure para alterar beneficiario.
- [x] Criar ou versionar procedure para excluir beneficiario.
- [x] Criar ou versionar procedure para verificar existencia de beneficiario duplicado para o mesmo cliente.
- [x] Atualizar a exclusao de cliente para remover os beneficiarios vinculados na mesma operacao.

Arquivos implementados nesta fase:
- `FI.AtividadeEntrevista/DAL/Clientes/Procedures/FI_ALTER_BENEFICIARIOS.sql`
- `FI.AtividadeEntrevista/DAL/Clientes/Procedures/FI_SP_ConsBeneficiarios.sql`
- `FI.AtividadeEntrevista/DAL/Clientes/Procedures/FI_SP_IncBeneficiario.sql`
- `FI.AtividadeEntrevista/DAL/Clientes/Procedures/FI_SP_AltBeneficiario.sql`
- `FI.AtividadeEntrevista/DAL/Clientes/Procedures/FI_SP_DelBeneficiario.sql`
- `FI.AtividadeEntrevista/DAL/Clientes/Procedures/FI_SP_VerificaBeneficiario.sql`
- `FI.AtividadeEntrevista/DAL/Clientes/Procedures/FI_SP_DelCliente.sql`
- `FI.AtividadeEntrevista/FI.AtividadeEntrevista.csproj`

## Fase 3: Backend

- [x] Criar DML para beneficiario.
- [x] Evoluir o modelo de cliente para transportar a colecao de beneficiarios no submit.
- [x] Carregar beneficiarios existentes no fluxo de alteracao do cliente.
- [x] Reutilizar a validacao de CPF existente para beneficiarios.
- [x] Validar obrigatoriedade de `CPF` e `Nome` dos beneficiarios no backend.
- [x] Validar duplicidade de CPF por cliente no backend usando o valor normalizado.
- [x] Persistir beneficiarios somente apos o salvamento do cliente.
- [x] Na alteracao, reconciliar inclusoes, alteracoes e exclusoes a partir da colecao enviada pela tela.

Arquivos implementados nesta fase:
- `FI.AtividadeEntrevista/DML/Beneficiario.cs`
- `FI.AtividadeEntrevista/DML/Cliente.cs`
- `FI.AtividadeEntrevista/BLL/BoCliente.cs`
- `FI.AtividadeEntrevista/DAL/Beneficiarios/DaoBeneficiario.cs`
- `FI.AtividadeEntrevista/DAL/Clientes/DaoCliente.cs`
- `FI.AtividadeEntrevista/DAL/Padrao/FI.AcessoDados.cs`
- `FI.AtividadeEntrevista/FI.AtividadeEntrevista.csproj`
- `FI.WebAtividadeEntrevista/Models/BeneficiarioModel.cs`
- `FI.WebAtividadeEntrevista/Models/ClienteModel.cs`
- `FI.WebAtividadeEntrevista/Controllers/ClienteController.cs`
- `FI.WebAtividadeEntrevista/FI.WebAtividadeEntrevista.csproj`

## Fase 4: Frontend

- [x] Adicionar o botao `Beneficiários` ao formulario compartilhado de cliente.
- [x] Criar o modal de manutencao com campos `CPF` e `Nome` e botao principal `Salvar`.
- [x] Criar o grid de beneficiarios dentro do modal exibindo `CPF` e `Nome`.
- [x] Posicionar o botao `Beneficiários` no lado oposto aos botoes `Salvar` e `Voltar`.
- [x] Permitir incluir beneficiarios em memoria antes do salvamento do cliente.
- [x] Permitir alterar um beneficiario preenchendo novamente os campos do modal.
- [x] Permitir excluir beneficiarios do grid em memoria com confirmacao.
- [x] Aplicar ao CPF do beneficiario a mesma mascara usada no CPF do cliente.
- [x] Impedir duplicidade de CPF no grid em memoria para o mesmo cliente exibindo a mensagem `CPF já incluído`.
- [x] Serializar a colecao de beneficiarios no submit do formulario principal.
- [x] Carregar no modal os beneficiarios existentes ao alterar um cliente.
- [x] Manter o rascunho de beneficiarios ao fechar e reabrir o pop-up na mesma tela.

Arquivos implementados nesta fase:
- `FI.WebAtividadeEntrevista/Views/Cliente/Forms.cshtml`
- `FI.WebAtividadeEntrevista/Scripts/Clientes/FI.Beneficiarios.js`
- `FI.WebAtividadeEntrevista/Scripts/Clientes/FI.Clientes.js`
- `FI.WebAtividadeEntrevista/Scripts/Clientes/FI.AltClientes.js`
- `FI.WebAtividadeEntrevista/App_Start/BundleConfig.cs`
- `FI.WebAtividadeEntrevista/FI.WebAtividadeEntrevista.csproj`

## Fase 5: Validacao e Documentacao

- [x] Validar manualmente a inclusao de cliente com um ou mais beneficiarios validos.
- [x] Validar manualmente a alteracao de cliente com inclusao de beneficiario.
- [x] Validar manualmente a alteracao de cliente com edicao de beneficiario.
- [x] Validar manualmente a alteracao de cliente com exclusao de beneficiario.
- [x] Validar manualmente o bloqueio de CPF duplicado para o mesmo cliente com a mensagem `CPF já incluído`.
- [x] Validar manualmente a permissao do mesmo CPF para clientes diferentes.
- [x] Validar manualmente que o cliente pode ser salvo sem beneficiarios.
- [x] Validar manualmente a confirmacao de exclusao de beneficiario no pop-up.
- [x] Validar manualmente que nenhuma operacao de beneficiario e persistida antes do clique em `Salvar` do cliente.
- [x] Validar manualmente a exclusao de cliente removendo tambem os beneficiarios vinculados.

Status final:
- [x] Fase 5 concluida.
- [x] Validacao manual executada.
- [x] Fluxos principais de inclusao, alteracao e manutencao de beneficiarios aprovados.
- [x] Regras de CPF, duplicidade e persistencia avaliadas manualmente.
- [x] Regressao do fluxo principal de cliente verificada.
