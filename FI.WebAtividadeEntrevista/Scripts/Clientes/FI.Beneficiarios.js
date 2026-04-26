var beneficiariosState = {
    itens: [],
    editandoId: 0,
    sequence: -1
};

function NormalizarCPF(value) {
    return (value || '').replace(/\D/g, '').substring(0, 11);
}

function MaskCPF(value) {
    value = NormalizarCPF(value);
    value = value.replace(/(\d{3})(\d)/, '$1.$2');
    value = value.replace(/(\d{3})(\d)/, '$1.$2');
    value = value.replace(/(\d{3})(\d{1,2})$/, '$1-$2');
    return value;
}

function ValidarCPF(value) {
    value = NormalizarCPF(value);

    if (value.length !== 11)
        return false;

    if (/^(\d)\1{10}$/.test(value))
        return false;

    var soma = 0;
    for (var i = 0; i < 9; i++)
        soma += parseInt(value.charAt(i), 10) * (10 - i);

    var resto = (soma * 10) % 11;
    if (resto === 10)
        resto = 0;

    if (resto !== parseInt(value.charAt(9), 10))
        return false;

    soma = 0;
    for (i = 0; i < 10; i++)
        soma += parseInt(value.charAt(i), 10) * (11 - i);

    resto = (soma * 10) % 11;
    if (resto === 10)
        resto = 0;

    return resto === parseInt(value.charAt(10), 10);
}

function BeneficiariosInit(initialItems) {
    beneficiariosState.itens = $.map(initialItems || [], function (item) {
        return {
            Id: item.Id || 0,
            CPF: NormalizarCPF(item.CPF),
            Nome: $.trim(item.Nome || '')
        };
    });

    beneficiariosState.editandoId = 0;
    bindBeneficiariosEvents();
    syncBeneficiariosPayload();
    renderBeneficiariosGrid();
}

function bindBeneficiariosEvents() {
    $('#btnBeneficiarios').off('click').on('click', function () {
        $('#modalBeneficiarios').modal('show');
    });

    $('#BeneficiarioCPF').off('input keyup change blur').on('input keyup change blur', function () {
        $(this).val(MaskCPF($(this).val()));
    });

    $('#btnSalvarBeneficiario').off('click').on('click', function () {
        salvarBeneficiario();
    });

    $('#modalBeneficiarios').off('hidden.bs.modal').on('hidden.bs.modal', function () {
        limparBeneficiarioForm();
    });
}

function salvarBeneficiario() {
    var cpf = NormalizarCPF($('#BeneficiarioCPF').val());
    var nome = $.trim($('#BeneficiarioNome').val());
    var id = beneficiariosState.editandoId;

    if (!cpf) {
        ModalDialog('Ocorreu um erro', 'O CPF do beneficiario é obrigatorio');
        return;
    }

    if (!ValidarCPF(cpf)) {
        ModalDialog('Ocorreu um erro', 'Digite um CPF valido para o beneficiario');
        return;
    }

    if (!nome) {
        ModalDialog('Ocorreu um erro', 'O nome do beneficiario é obrigatorio');
        return;
    }

    if (beneficiariosState.itens.some(function (item) {
        return item.Id !== id && item.CPF === cpf;
    })) {
        ModalDialog('Ocorreu um erro', 'CPF já incluído');
        return;
    }

    if (id === 0) {
        beneficiariosState.itens.push({
            Id: beneficiariosState.sequence--,
            CPF: cpf,
            Nome: nome
        });
    }
    else {
        var beneficiario = getBeneficiarioById(id);
        if (!beneficiario) {
            ModalDialog('Ocorreu um erro', 'Beneficiario invalido para o cliente informado');
            return;
        }

        beneficiario.CPF = cpf;
        beneficiario.Nome = nome;
    }

    syncBeneficiariosPayload();
    renderBeneficiariosGrid();
    limparBeneficiarioForm();
}

function renderBeneficiariosGrid() {
    var tbody = $('#gridBeneficiarios tbody');
    tbody.empty();

    if (!beneficiariosState.itens.length) {
        tbody.append('<tr class="beneficiario-empty"><td colspan="3" class="text-center">Nenhum beneficiário adicionado</td></tr>');
        return;
    }

    $.each(beneficiariosState.itens, function (_, item) {
        var row = $('<tr />');
        row.append($('<td />').text(MaskCPF(item.CPF)));
        row.append($('<td />').text(item.Nome));

        var actions = $('<td class="text-right" />');
        actions.append($('<button type="button" class="btn btn-sm btn-primary" style="margin-right: 5px;">Alterar</button>').on('click', function () {
            editarBeneficiario(item.Id);
        }));
        actions.append($('<button type="button" class="btn btn-sm btn-danger">Excluir</button>').on('click', function () {
            excluirBeneficiario(item.Id);
        }));
        row.append(actions);

        tbody.append(row);
    });
}

function editarBeneficiario(id) {
    var beneficiario = getBeneficiarioById(id);
    if (!beneficiario)
        return;

    beneficiariosState.editandoId = beneficiario.Id;
    $('#BeneficiarioCPF').val(MaskCPF(beneficiario.CPF));
    $('#BeneficiarioNome').val(beneficiario.Nome);
    $('#modalBeneficiarios').modal('show');
    $('#BeneficiarioCPF').focus();
}

function excluirBeneficiario(id) {
    var beneficiario = getBeneficiarioById(id);
    if (!beneficiario)
        return;

    if (!window.confirm('Deseja excluir o beneficiário selecionado?'))
        return;

    beneficiariosState.itens = $.grep(beneficiariosState.itens, function (item) {
        return item.Id !== id;
    });

    syncBeneficiariosPayload();
    renderBeneficiariosGrid();
    limparBeneficiarioForm();
}

function limparBeneficiarioForm() {
    beneficiariosState.editandoId = 0;
    $('#BeneficiarioCPF').val('');
    $('#BeneficiarioNome').val('');
}

function syncBeneficiariosPayload() {
    var payload = $.map(beneficiariosState.itens, function (item) {
        return {
            Id: item.Id > 0 ? item.Id : 0,
            CPF: item.CPF,
            Nome: item.Nome
        };
    });

    $('#BeneficiariosJson').val(JSON.stringify(payload));
}

function getBeneficiarioById(id) {
    for (var i = 0; i < beneficiariosState.itens.length; i++) {
        if (beneficiariosState.itens[i].Id === id)
            return beneficiariosState.itens[i];
    }

    return null;
}
