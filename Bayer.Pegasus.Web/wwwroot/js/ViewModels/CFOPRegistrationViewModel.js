function CFOPRegistrationViewModel(dashboard) {
    var self = this;
    /* 28-01-2019 
     * Criação do datatable.js para os dados da CFOP
     * dom: 'B<"clear"><f><rt><ip>',
     */

    self.Cd_Cfop = ko.observable("");
    self.Cd_CfopHidden = ko.observable("");
    self.Ds_Cfop = ko.observable("");
    self.OperationType = ko.observable("Não Definido");
    self.Fl_Pegasus = ko.observable(false);
    self.Fl_Ativo = ko.observable(false);
    self.Acao = ko.observable("");
    self.isDisabled = ko.observable(false);
    self.OperationType = ko.observable("DÉBITO");    
    self.TitleModal = ko.observable("");

    $("#IdCodigoCfop").mask("9999");
    $('.container div').delay(2000).fadeOut("slow");

    var dt = $('#DatatableCFOPRegistration').DataTable({
        "dom": 'B<"clear"><f><rt><ip>',
        "bLengthChange": true,
        "bPaginate": true,
        "bFilter": true,
        "bInfo": true,
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        buttons: [
            {
                text: '<i class="fa fa-plus-square text-primary"></i>',
                titleAttr: "Adicionar novo registro",
                action: function (e, dt, node, config) {
                    $("#modalCFOP").modal('show');                       
                    self.LimparCampos();
                    self.Acao("I");
                    self.TitleModal("Cadastro de Cfop");
                }
            }],
        columns: [
            { data: 'Cd_Cfop', title: "CFOP" },
            { data: 'Ds_Cfop', title: "Descrição" },
            { data: 'Fl_Operacao', title: "Operação" },
            { data: 'Fl_Pegasus', title: "Escopo Pegasus" },
            { data: 'Fl_Ativo', title: "Situação" },
            {
                title: "Editar"
            }
        ],
        "columnDefs": [
            { width: "10%", "targets": [0] },
            { width: "45%", "targets": [1] },
            { width: "15%", "targets": [2] },
            { width: "15%", "targets": [3] },
            { width: "15%", "targets": [4] },
            {
                width: "10%",
                "searchable": false,
                className: "actions",
                "render": function (data, type, row) {
                    var btnString = '<button type="button" class="btn btn btn-cyan" data-toggle="modal" data-target="#modalCFOP" title="Editar registro"><i class="fa fa-edit"></i></button>';
                    return btnString;
                },
                targets: 5
            }
        ]
        //,
        //"language": {
        //    "url": "../../json/Portuguese-Brasil.json"
        // }
    });

    // DatatableCFOPRegistration object
    var CFOPRegistration = function (data, dt) {
        this.Cd_Cfop = ko.observable(data.Cd_Cfop);
        this.Ds_Cfop = ko.observable(data.Ds_Cfop);
        this.Fl_Operacao = ko.observable((data.Fl_Operacao == "-1" ? "Débito" : (data.Fl_Operacao == "0" ? "Não definido" : "Crédito")));
        this.Fl_Pegasus = ko.observable(data.Fl_Pegasus == true ? "Ativo" : "Inativo");
        this.Fl_Ativo = ko.observable(data.Fl_Ativo == true ? "Ativo" : "Inativo");

        // Subscribe a listener to the observable properties for the table
        // and invalidate the DataTables row when they change so it will redraw
        var that = this;
        $.each(['Cd_Cfop', 'Ds_Cfop', 'Fl_Operacao', 'Fl_Pegasus', 'Fl_Ativo'], function (i, prop) {
            that[prop].subscribe(function (val) {
                // Find the row in the DataTable and invalidate it, which will
                // cause DataTables to re-read the data
                var rowIdx = dt.column(0).data().indexOf(that.Cd_Cfop);
                dt.row(rowIdx).invalidate();
            });
        });
    };

    ko.observableArray.fn.subscribeArrayChanged = function (addCallback, deleteCallback) {
        var previousValue = undefined;
        this.subscribe(function (_previousValue) {
            previousValue = _previousValue.slice(0);
        }, undefined, 'beforeChange');
        this.subscribe(function (latestValue) {
            var editScript = ko.utils.compareArrays(previousValue, latestValue);
            for (var i = 0, j = editScript.length; i < j; i++) {
                switch (editScript[i].status) {
                    case "retained":
                        break;
                    case "deleted":
                        if (deleteCallback)
                            deleteCallback(editScript[i].value);
                        break;
                    case "added":
                        if (addCallback)
                            addCallback(editScript[i].value);
                        break;
                }
            }
            previousValue = undefined;
        });
    };

    ko.bindingHandlers.selectPicker = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            if ($(element).is('select')) {
                if (ko.isObservable(valueAccessor())) {
                    if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
                        // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
                        ko.bindingHandlers.selectedOptions.init(element, valueAccessor, allBindingsAccessor);
                    } else {
                        // regular select and observable so call the default value binding
                        ko.bindingHandlers.value.init(element, valueAccessor, allBindingsAccessor);

                    }
                }
                $(element).addClass('selectpicker').selectpicker();
            }
        },
        update: function (element, valueAccessor, allBindingsAccessor) {
            if ($(element).is('select')) {
                var selectPickerOptions = allBindingsAccessor().selectPickerOptions;
                if (typeof selectPickerOptions !== 'undefined' && selectPickerOptions !== null) {
                    var options = selectPickerOptions.optionsArray,
                        optionsText = selectPickerOptions.optionsText,
                        optionsValue = selectPickerOptions.optionsValue,
                        optionsCaption = selectPickerOptions.optionsCaption,
                        isDisabled = selectPickerOptions.disabledCondition || false,
                        resetOnDisabled = selectPickerOptions.resetOnDisabled || false;
                    if (ko.utils.unwrapObservable(options).length > 0) {
                        // call the default Knockout options binding
                        ko.bindingHandlers.options.update(element, options, allBindingsAccessor);
                    }
                    if (isDisabled && resetOnDisabled) {
                        // the dropdown is disabled and we need to reset it to its first option
                        $(element).selectpicker('val', $(element).children('option:first').val());
                    }
                    $(element).prop('disabled', isDisabled);
                }
                if (ko.isObservable(valueAccessor())) {
                    if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
                        // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
                        ko.bindingHandlers.selectedOptions.update(element, valueAccessor);
                    } else {
                        // call the default Knockout value binding
                        ko.bindingHandlers.value.update(element, valueAccessor);

                    }
                }

                $(element).selectpicker('refresh');
            }
        }
    };

    self.GetListCFOPRegistration = function () {
        $.ajax({
            url: util.mapUrl("/CFOPRegistration/GetListCFOPRegistration"),
            contentType: false,
            processData: false,
            method: 'GET',
            success: function (results) {

                if (results.hasErrors) {
                    util.ValidationResults.ShowErrors(results);
                }
                else {

                    var st = ko.mapping.fromJS([]);
                    var dataJson = JSON.parse(results.dataTable);
                    //alert(dataJson);
                    //alert(JSON.stringify(dataJson));
                    // Update the table when the `people` array has items added or removed
                    st.subscribeArrayChanged(
                        function (addedItem) {
                            dt.row.add(addedItem).draw();
                        },
                        function (deletedItem) {
                            var rowIdx = dt.column(0).data().indexOf(deletedItem.Cd_Cfop);
                            dt.row(rowIdx).remove().draw();
                        }
                    );

                    // Convert the data set into observable objects, and will also add the
                    // initial data to the table
                    ko.mapping.fromJS(
                        dataJson,
                        {
                            key: function (dataJson) {
                                return ko.utils.unwrapObservable(dataJson.Cd_Cfop);
                            },
                            create: function (options) {
                                return new CFOPRegistration(options.data, dt);
                            }
                        },
                        st
                    );
                }
            }
        });
    };

    // Setup - add a text input to each footer cell
    $('#DatatableCFOPRegistration thead tr').clone(true).appendTo('#DatatableCFOPRegistration thead');
    $('#DatatableCFOPRegistration thead tr:eq(1) th').each(function (i) {
        var title = $(this).text();
        if (title) {
            $(this).html('<input type="text" placeholder="Filtrar:" style="padding-left: 5px;" />');

            $('input', this).on('keyup change', function () {
                if (dt.column(i).search() !== this.value) {
                    dt
                        .column(i)
                        .search(this.value)
                        .draw();
                }
            });
        }
    });

    $('#DatatableCFOPRegistration tbody').on('click', 'td', function () {
        //Se for Exportar Excel
        //$("#modalCFOP").modal();
        if (dt.cell(this).index().column == 5) {
            var codCfop = parseInt(dt.cell(dt.cell(this).index().row, 0).data());
            var descCfop = dt.cell(dt.cell(this).index().row, 1).data();
            var operacao = dt.cell(dt.cell(this).index().row, 2).data().toUpperCase();
            var escopoPegasus = dt.cell(dt.cell(this).index().row, 3).data();
            var situacao = dt.cell(dt.cell(this).index().row, 4).data();
            var acao = "U";

            var myObject = new Object();
            myObject.Cd_Cfop = parseInt(dt.cell(dt.cell(this).index().row, 0).data());
            myObject.Ds_Cfop = dt.cell(dt.cell(this).index().row, 1).data();
            myObject.Fl_Operacao = dt.cell(dt.cell(this).index().row, 2).data().toUpperCase();
            myObject.Fl_Pegasus = dt.cell(dt.cell(this).index().row, 3).data();
            myObject.Fl_Ativo = dt.cell(dt.cell(this).index().row, 4).data();
            myObject.acao = "U";

            var result = JSON.stringify(myObject);

            if (codCfop > 0) {
                self.TitleModal("Editar Cfop");
                self.Cd_Cfop(codCfop);
                self.Cd_CfopHidden(codCfop);
                self.Ds_Cfop(descCfop);
                self.Acao(acao);
                self.Fl_Pegasus(escopoPegasus == "Ativo" ? true : false);
                self.Fl_Ativo(situacao == "Ativo" ? true : false);
                self.isDisabled(true);
                switch (operacao) {
                    case 'DÉBITO':
                        self.OperationType(operacao);
                        break;
                    case 'NÃO DEFINIDO':
                        self.OperationType(operacao);
                        break;
                    case 'CRÉDITO':
                        self.OperationType(operacao);
                        break;
                    default:
                        self.OperationType(operacao);
                        break;
                }
            }
        }
    });

    self.CreateOrUpdateCFOP = function () {
        var CfopRegistration = {};
        CfopRegistration.Cd_Cfop = (self.Cd_Cfop() == "" ? 0 : self.Cd_Cfop());
        CfopRegistration.Ds_Cfop = self.Ds_Cfop();
        CfopRegistration.Fl_Operacao = (self.OperationType() == "DÉBITO" ? -1 : (self.OperationType() == "NÃO DEFINIDO" ? 0 : 1));
        CfopRegistration.Fl_Pegasus = self.Fl_Pegasus();
        CfopRegistration.Fl_Ativo = self.Fl_Ativo();
        CfopRegistration.Acao = self.Acao();
        var params = JSON.stringify(CfopRegistration);
       
        $.ajax({
            type: "POST",
            url: util.mapUrl("/CFOPRegistration/Save"),
            data: params,
            contentType: "application/json",

            success: function (results) {

                if (results.hasErrors) {
                    util.ValidationResults.ShowErrors(results);
                } else {
                    self.LimparCampos();
                    $('#modalCFOP').modal('toggle');
                    $('#DatatableCFOPRegistration').DataTable().clear().draw();
                    self.GetListCFOPRegistration();
                    util.ShowSuccessMessage('Gravado com Sucesso!');
                }

            },
            error: function (xhr, status, p3, p4) {
                var err = "Error " + " " + status + " " + p3 + " " + p4;
                if (xhr.responseText && xhr.responseText[0] == "{")
                    err = JSON.parse(xhr.responseText).Message;
                console.log(err);
            }
        });

    };
    self.LimparCampos = function () {
        self.Cd_Cfop("0");
        self.Cd_CfopHidden("0");
        self.isDisabled(false);
        self.Ds_Cfop("");
        self.OperationType("DÉBITO");
        self.Fl_Pegasus(false);
        self.Fl_Ativo(false);
        self.Acao(" ");
    };
}