# Tasks: Campo CPF do Cliente

## Fase 1: Decisoes e Preparacao

- [x] Definir se o CPF sera salvo com mascara ou somente com numeros.

## Fase 2: Banco de Dados

- [x] Criar script SQL para adicionar a coluna `CPF` na tabela `CLIENTES`.
- [x] Criar indice unico para `CLIENTES.CPF`.
- [x] Atualizar `FI_SP_IncClienteV2` para receber e inserir CPF.
- [x] Atualizar `FI_SP_AltCliente` para receber e alterar CPF.
- [x] Atualizar `FI_SP_ConsCliente` para retornar CPF.
- [x] Atualizar `FI_SP_PesqCliente` para retornar CPF quando necessario.
- [x] Criar/versionar `FI_SP_VerificaCliente` para consultar CPF existente.

## Fase 3: Backend

- [x] Adicionar propriedade `CPF` em `DML.Cliente`.
- [x] Adicionar propriedade `CPF` em `ClienteModel` com `Required` e validacao customizada.
- [x] Criar validador de CPF reutilizavel no backend.
- [x] Mapear CPF nos metodos `Incluir`, `Alterar` e `Alterar(long id)` do controller.
- [x] Atualizar `DaoCliente` para enviar e ler CPF.

## Fase 4: Frontend

- [x] Inserir o campo CPF em `Forms.cshtml` abaixo de `Nome`.
- [x] Atualizar scripts de cadastro e alteracao para enviar/preencher CPF.
- [x] Aplicar mascara `999.999.999-99` no frontend.

## Fase 5: Validacao e Documentacao

- [x] Validar manualmente cadastro com CPF invalido.
- [x] Validar manualmente cadastro com CPF duplicado.
- [x] Validar manualmente cadastro com CPF valido.
- [x] Validar manualmente alteracao de cliente com CPF.
