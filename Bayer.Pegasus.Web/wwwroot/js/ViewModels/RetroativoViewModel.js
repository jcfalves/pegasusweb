function RetroativoViewModel(dashboard) {
    var self = this;
    var idArquivos = [];
    self.idArquivoretroativo = ko.observable("");
    self.idArquivoretroativoHidden = ko.observable("");
    self.dsNome = ko.observable("");
    self.idAcao = ko.observable("");
    self.dsAcao = ko.observable("Pendente");
    self.dtAcao = ko.observable("");
    self.TitleModal = ko.observable("Editando Arquivo retroativo");

    $('.container div').delay(2000).fadeOut("slow");

    var dt = $('#DatatableCFOPRegistration').DataTable({
        "dom": 'B<"clear"><f><rt><ip>',
        "bLengthChange": true,
        "bPaginate": true,
        "bFilter": true,
        "bInfo": true,
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        buttons: [],
        columns: [
            { data: null },
            { data: 'idArquivoretroativo' },
            { data: 'dsNome' },
            { data: 'dsStatus' },
            { data: 'dsAcao' }, 
            { data: 'dtAcao' }
        ],
        select: {
            style: 'multi',
            selector: 'td:first-child'            
        },
        columnDefs: [
            {
                targets: 0,                                
                checkboxes: { selectRow: true }
            },
            { targets: 1, title: 'ID Arquivo', orderable: true, width: '10%' },
            { targets: 2, title: 'Arquivo', orderable: true, width: '51%' },
            { targets: 3, title: 'Status', orderable: true, width: '15%' },
            { targets: 4, title: 'Ação', orderable: true, width: '10%' },
            { targets: 5, title: 'Data da Ação', orderable: true, width: '14%' }
        ]
    });

    // DatatableCFOPRegistration object
    var CFOPRegistration = function (data, dt) {
        this.idArquivoretroativo = ko.observable(data.idArquivoretroativo);
        this.dsAcao = ko.observable(data.dsAcao);
        this.dsNome = ko.observable(data.dsNome);
        this.dsStatus = ko.observable(data.dsStatus);
        this.dtAcao = ko.observable(data.dtAcao);

        // Subscribe a listener to the observable properties for the table
        // and invalidate the DataTables row when they change so it will redraw
        var that = this;
        $.each(['idArquivoretroativo', 'dsNome', 'dsStatus', 'dsAcao', 'dtAcao'], function (i, prop) {
            that[prop].subscribe(function (val) {
                // Find the row in the DataTable and invalidate it, which will
                // cause DataTables to re-read the data
                var rowIdx = dt.column(0).data().indexOf(that.idArquivoretroativo);
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


    self.GetListArquivosRetroativos = function () {

        $('#ModalSpinner').modal('show');

        var Status = '';

        if (document.getElementById("statusacao").value == "") {
            Status = { status: 'pendentes' };
        } else {
            Status = { status: document.getElementById("statusacao").value };
        }

        $.ajax({
            url: util.mapUrl("/Retroativo/GetListArquivosRetroativos"),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            method: 'POST',
            data: JSON.stringify(Status),

            success: function (results) {

                $('#ModalSpinner').modal('hide');

                if (results.hasErrors) {
                    util.ValidationResults.ShowErrors(results);
                }
                else {
                    $('#DatatableCFOPRegistration').DataTable().clear().draw();

                    var st = ko.mapping.fromJS([]);
                    var dataJson = JSON.parse(results.dataTable);

                    // Update the table when the `people` array has items added or removed
                    st.subscribeArrayChanged(
                        function (addedItem) {
                            dt.row.add(addedItem).draw();
                        },
                        function (deletedItem) {
                            var rowIdx = dt.column(0).data().indexOf(deletedItem.idArquivoretroativo);
                            dt.row(rowIdx).remove().draw();
                        }
                    );

                    // Convert the data set into observable objects, and will also add the
                    // initial data to the table
                    ko.mapping.fromJS(
                        dataJson,
                        {
                            key: function (dataJson) {
                                return ko.utils.unwrapObservable(dataJson.idArquivoretroativo);
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



    self.OnGenerateExcel = function () {

        
        var Status = '';

        if (document.getElementById("statusacao").value == "") {
            Status = { status: 'pendentes' };
        } else {
            Status = { status: document.getElementById("statusacao").value };
        }

        $("#LoadingExcel").show();
        $("#LoadedExcel").hide();
        $('#ModalExcel').modal('show');

        $.ajax({
            type: "POST",
            url: util.mapUrl("/Retroativo/GenerateExcel"),
            data: JSON.stringify(Status),
            contentType: "application/json",
            success: function (results) {

                if (results.hasErrors) {
                    $('#ModalExcel').modal('hide');
                    util.ValidationResults.ShowErrors(results);
                }
                else {
                    var identifier = results.Identifier;

                    var url = util.mapUrl("/Retroativo/Download/?identifier=" + identifier);

                    $("#LoadedExcel #linkExcelDownloaded").attr("href", url)

                    $("#LoadingExcel").hide();
                    $("#LoadedExcel").show();

                }
            }
        });
        
    }

    //Setup - add a text input to each footer cell
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


    self.CreateOrUpdateCFOP = function () {
        var arquivoRetroativo = {};
        arquivoRetroativo.idArquivoretroativo = (self.idArquivoretroativo() == "" ? 0 : self.idArquivoretroativo());
        arquivoRetroativo.dsAcao = self.dsAcao();
        arquivoRetroativo.dsNome = self.dsNome();
        arquivoRetroativo.idAcao = 1; //self.idAcao();
        arquivoRetroativo.dtAcao = self.dtAcao();
        var params = JSON.stringify(arquivoRetroativo);

        $.ajax({
            type: "POST",
            url: util.mapUrl("/Retroativo/Save"),
            data: params,
            contentType: "application/json",

            success: function (results) {

                if (results.hasErrors) {
                    util.ValidationResults.ShowErrors(results);
                } else {
                    self.LimparCampos();
                    $('#DatatableCFOPRegistration').DataTable().clear().draw();
                    self.GetListArquivosRetroativos();
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

    self.CreateOrUpdateCFOP1 = function () {
        var arquivoRetroativo = {};
        arquivoRetroativo.idArquivoretroativo = (self.idArquivoretroativo() == "" ? 0 : self.idArquivoretroativo());
        arquivoRetroativo.dsAcao = self.dsAcao();
        arquivoRetroativo.dsNome = self.dsNome();
        arquivoRetroativo.idAcao = 2; //self.idAcao();
        arquivoRetroativo.dtAcao = self.dtAcao();
        var params = JSON.stringify(arquivoRetroativo);

        $.ajax({
            type: "POST",
            url: util.mapUrl("/Retroativo/Save"),
            data: params,
            contentType: "application/json",

            success: function (results) {

                if (results.hasErrors) {
                    util.ValidationResults.ShowErrors(results);
                } else {
                    self.LimparCampos();
                    $('#DatatableCFOPRegistration').DataTable().clear().draw();
                    self.GetListArquivosRetroativos();
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

    self.CreateOrUpdateCFOP2 = function () {
        var arquivoRetroativo = {};
        arquivoRetroativo.idArquivoretroativo = (self.idArquivoretroativo() == "" ? 0 : self.idArquivoretroativo());
        arquivoRetroativo.dsAcao = self.dsAcao();
        arquivoRetroativo.dsNome = self.dsNome();
        arquivoRetroativo.idAcao = 3; //self.idAcao();
        arquivoRetroativo.dtAcao = self.dtAcao();
        var params = JSON.stringify(arquivoRetroativo);

        $.ajax({
            type: "POST",
            url: util.mapUrl("/Retroativo/Save"),
            data: params,
            contentType: "application/json",

            success: function (results) {

                if (results.hasErrors) {
                    util.ValidationResults.ShowErrors(results);
                } else {
                    self.LimparCampos();
                    $('#DatatableCFOPRegistration').DataTable().clear().draw();
                    self.GetListArquivosRetroativos();
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
        self.idArquivoretroativo("0");
        self.idArquivoretroativoHidden("0");
        self.dsAcao("");
        self.dsNome("");
        self.dtAcao("");
    };


    self.AcaoArquivos = function (data) {

        var arrIdArquivos = [];
        var collection = dt.rows('.selected').data();
        $.each(collection, function () {
            arrIdArquivos.push(ko.utils.unwrapObservable(this['idArquivoretroativo']));
        });

        if (arrIdArquivos.length == 0) {
            util.ShowWarning("Selecione pelo menos um arquivo!");
            return;
        }

        var idAcao = parseInt(data);
        var msgRetorno = "";

        switch (idAcao) {
            case 1: //Processamento Total
                msgRetorno = "Processamento Total gravado com Sucesso!";
                break;

            case 2: //Processamento Parcial
                msgRetorno = "Processamento Parcial gravado com Sucesso!";
                break;

            case 3: //Exclusão
                msgRetorno = "Exclusão gravada com Sucesso!";
                break;

            case 4: //Cancelamento
                msgRetorno = "Cancelamento gravado com Sucesso!";
                break;

            default:
                msgRetorno = "";
        }

        var params = JSON.stringify({
            idAcao: idAcao, arrIdArquivos: arrIdArquivos
        });

        $.ajax({
            type: "POST",
            url: util.mapUrl("/Retroativo/AcaoArquivos"),
            data: params,
            contentType: "application/json",
            success: function (results) {

                if (results.hasErrors) {
                    util.ValidationResults.ShowErrors(results);
                }
                else {
                    self.LimparCampos();
                    $('#DatatableCFOPRegistration').DataTable().clear().draw();
                    self.GetListArquivosRetroativos();
                }
            }
        });
    };

}