function HealthCheckViewModel(dashboard) {
    //desabilita o chart
    $('#DashBoard').hide();
    var selectALL = false;

    var dt = $('#dt_hc_type_error').DataTable({
        "bLengthChange": false,
        "bPaginate": false,
        "bFilter": false,
        "bInfo": false,
        columns: [
            { data: 'Cd_Erro', title: "Codigo" },
            { data: 'Id_Categoria_Erro', title: "Categoria Erro" },
            { data: 'Ds_Erro', title: "Descrição" },
            { data: 'Qt_Ocorrencia', title: "Quantidade" },
            { data: 'Pc_Ocorrencia_Total', title: "%" }
        ]
        //,
        //"language": {
           // "url": "../../json/Portuguese-Brasil.json"
        //}
    });
    
    //Validação de Email
    function ValidateEmail(email) {
        var expr = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
        return expr.test(email);
    };

    // HealthCheck object
    var HealthCheck = function (data, dt) {
        this.Cd_Erro = ko.observable(data.Cd_Erro);
        this.Ds_Erro = ko.observable(data.Ds_Erro);
        this.Id_Categoria_Erro = ko.observable(data.Id_Categoria_Erro);
        this.Qt_Ocorrencia = ko.observable(data.Qt_Ocorrencia);
        this.Pc_Ocorrencia_Total = ko.observable(data.Pc_Ocorrencia_Total);

        // Subscribe a listener to the observable properties for the table
        // and invalidate the DataTables row when they change so it will redraw
        var that = this;
        $.each(['Cd_Erro','Pc_Ocorrencia_Total','Qt_Ocorrencia', 'Ds_Erro', 'Id_Categoria_Erro'], function (i, prop) {
            that[prop].subscribe(function (val) {
                // Find the row in the DataTable and invalidate it, which will
                // cause DataTables to re-read the data
                var rowIdx = dt.column(0).data().indexOf(that.Cd_Erro);
                dt.row(rowIdx).invalidate();
            });
        });
    };

    var self = this;

    self.EmailTo = ko.observable("");
    
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

    self.GetTypeErrorHealthCheck = function () {
        $.ajax({
            url: util.mapUrl("/HealthCheck/GetTypeErrorHealthCheck"),
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

                    // Update the table when the `people` array has items added or removed
                    st.subscribeArrayChanged(
                        function (addedItem) {
                            dt.row.add(addedItem).draw();
                        },
                        function (deletedItem) {
                            var rowIdx = dt.column(0).data().indexOf(deletedItem.Cd_Erro);
                            dt.row(rowIdx).remove().draw();
                        }
                    );

                    // Convert the data set into observable objects, and will also add the
                    // initial data to the table
                    ko.mapping.fromJS(
                        dataJson,
                        {
                            key: function (dataJson) {
                                return ko.utils.unwrapObservable(dataJson.Cd_Erro);
                            },
                            create: function (options) {
                                return new HealthCheck(options.data, dt);
                            }
                        },
                        st
                    );
                }

            }
        });
    };

    self.ShowModalEmail = function () {

        $("#EmailTo").val("");
        $('#ModalEmailGenerate').modal();
    }

    self.ProcessEmail = function () {

        if (!ko.toJS(self).EmailTo) {
            util.ShowWarning("Insira um e-mail!");
            return;
        }

        if (!ValidateEmail(ko.toJS(self).EmailTo)) {
            util.ShowWarning("Insira um e-mail válido!");
            $("#EmailTo").val("");
            return;
        }

        $('#ModalEmailGenerate').modal('hide');
        self.GetErrorHealthCheck();
    } 

    self.GetErrorHealthCheck = function () {

        $("#ModalLoadingHC").modal();

        util.ValidationResults.ClearErrors();

        var dateFilterIni = $("#StartDate").val();
        if (dateFilterIni == '') {
            util.ShowWarning("Selecione uma data de início!");
            return;
        }
         
        var dateFilterEnd = $("#EndDate").val();
        if (dateFilterEnd == '') {
            util.ShowWarning("Selecione uma data de término!");
            return;
        }

        var categoryIdSelected = $("#dropDownListCategory option:selected").val();
        if (categoryIdSelected == '') {
            util.ShowWarning("Selecione uma categoria!");
            return;
        }

        var listCheckes = ko.toJSON(self.checkedTypes)
        if (listCheckes == '[]') {
            util.ShowWarning("Selecione pelo menos um Tipo de erro!");
            return;
        }

        //var cEmail = ko.toJS(self).EmailTo;
        //if (!cEmail) {
        //    util.ShowWarning("Insira um e-mail!");
        //    $("#ModalLoadingHC").modal('hide');
        //    $('#ModalEmailGenerate').modal();
        //    return;

        //} 
        //$('#ModalEmailGenerate').modal('hide'); 

        var params = JSON.stringify({
                //Email: ko.toJS(self).EmailTo,
                DateIni: dateFilterIni,
                DateEnd: dateFilterEnd,
                Id_Categoria_Erro: categoryIdSelected,
                CheckedTypes: listCheckes
            });

        $.ajax({
            url: util.mapUrl("/HealthCheck/GetErrorHealthCheck"),
            method: 'POST',
            data: params,
            contentType: "application/json",
            success: function (results) {

                $('#ModalLoadingHC').modal('toggle');

                if (results.hasErrors) {
                    util.ValidationResults.ShowErrors(results);
                }
                else {
                    //util.ShowSuccessMessage("Seu arquivo será processado, e após término, será enviado por e-mail!");
                    $("#ModalLoadingHC").modal('hide');
                    if (results.Identifier) {
                        var identifier = results.Identifier;
                        var url = util.mapUrl("/HealthCheck/Download/?identifier=" + identifier);
                        window.open(url, '_blank');
                    } else {
                        util.ShowWarning("Não foi possível gerar o excel!");
                    }


                    //$("#EmailTo").val("");
                    //self.EmailTo = ko.observable(""); 
                }
            },
            error: function (xhr, status, p3, p4) {
                var err = "Error " + " " + status + " " + p3 + " " + p4;
                if (xhr.responseText && xhr.responseText[0] == "{")
                    err = JSON.parse(xhr.responseText).Message;                
            }
        });
    };

    self.dropDownListSelCategory = ko.observableArray();
    self.dropDownListSelTypeError = ko.observableArray();

    self.valuesForCheckBoxList = ko.observableArray([]);
    self.checkedTypes = ko.observableArray();
    self.KOteamMemberID = ko.observable();
    self.BSteamMemberID = ko.observable();

   self.GetListCategoryHeathCheck = function () {
        self.dropDownListSelCategory.removeAll();
        $.ajax({
            url: util.mapUrl("/HealthCheck/GetListCategoryHeathCheck"),
            contentType: false,
            processData: false,
            method: 'GET',
            success: function (result) {
                var category = JSON.parse(result.dataTable);
                //self.dropDownListSelCategory.push({ name: 'Todos', id: '0' });
                $.each(category, function (index, categorytype) {
                    var item = {
                        name: categorytype.Ds_Categoria_Erro,
                        id: categorytype.Id_Categoria_Erro
                    };
                    self.dropDownListSelCategory.push(item);
                });
            },
            error: function (xhr, status, p3, p4) {
                var err = "Error " + " " + status + " " + p3 + " " + p4;
                if (xhr.responseText && xhr.responseText[0] == "{")
                    err = JSON.parse(xhr.responseText).Message;
                console.log(err);
            }
        });
    };

    self.allSelected = ko.observable(false);

    self.selectAll = function () {

        if (selectALL) {
            self.checkedTypes.removeAll();
            ko.utils.arrayForEach(this.valuesForCheckBoxList(), function (item) {
                self.checkedTypes.push(item.Id)
            });

            selectALL = false;
        } else {
            self.checkedTypes.removeAll();
            selectALL = true;
        }
    }

    self.categoryIdSelected = function () {

        var categoryIdSelected = $("#dropDownListCategory option:selected").val();
        if (categoryIdSelected) {
            var params = JSON.stringify({ Id_Categoria_Erro: categoryIdSelected });
            selectALL = true;
            $.ajax({
                method: 'POST',
                url: util.mapUrl("/HealthCheck/GetTypeErrorCategoryIdHC"),
                data: params,
                contentType: "application/json",
                success: function (result) {
                    var listTypeError = JSON.parse(result.dataTable);
                    if (listTypeError.length > 0) {
                        self.allSelected(false);
                        self.checkedTypes.removeAll();
                        self.valuesForCheckBoxList.removeAll();
                        $("#ToDoListError").show();
                        $.each(listTypeError, function (index, typeError) {
                            var item = {
                                Name: typeError.Ds_Erro,
                                Id: typeError.Cd_Erro,
                                isSelected: ko.observable(false)
                            }
                            self.valuesForCheckBoxList.push(item);
                        });

                    } else {
                        $("#ToDoListError").hide();
                    }
                },
                error: function (xhr, status, p3, p4) {
                    var err = "Error " + " " + status + " " + p3 + " " + p4;
                    if (xhr.responseText && xhr.responseText[0] == "{")
                        err = JSON.parse(xhr.responseText).Message;
                    console.log(err);
                }
            });
        } else {
            $("#ToDoListError").hide();
        }
    } 

    self.GenerateTable = function () {

        util.ValidationResults.ClearErrors();

        var dateFilterIni = $("#StartDate").val();
        if (dateFilterIni == '') {
            util.ShowWarning("Selecione uma data de início!");
            return;
        }
         
        var dateFilterEnd = $("#EndDate").val();
        if (dateFilterEnd == '') {
            util.ShowWarning("Selecione uma data de término!");
            return;
        }

        var categoryIdSelected = $("#dropDownListCategory option:selected").val();
        if (categoryIdSelected == '') {
            util.ShowWarning("Selecione uma categoria!");
            return;
        }

        var listCheckes = ko.toJSON(self.checkedTypes)
        if (listCheckes == '[]') {
            util.ShowWarning("Selecione pelo menos um Tipo de erro!");
            return;
        }

        if (categoryIdSelected) {

            $("#ModalLoadingHC").modal();

            var params = JSON.stringify({
                DateIni: dateFilterIni,
                DateEnd: dateFilterEnd,
                Id_Categoria_Erro: categoryIdSelected,
                CheckedTypes: listCheckes
            });
            $.ajax({
                method: 'POST',
                url: util.mapUrl("/HealthCheck/GetTypeErrorHealthCheck"),
                data: params,
                contentType: "application/json",
                success: function (results) {

                    $('#ModalLoadingHC').modal('toggle');
                    
                    if (results.hasErrors) {
                        util.ValidationResults.ShowErrors(results);
                    }
                    else {

                        dt.clear().draw();
                        //$("#ModalResultTable").modal();
                        //oculta o filtro e habilita o dash
                        $('#filtro-consulta').hide();
                        $('#DashBoard').show();
                        
                        var st = ko.mapping.fromJS([]);
                        var dataJson = JSON.parse(results.dataTable);

                        // Update the table when the `people` array has items added or removed
                        st.subscribeArrayChanged(
                            function (addedItem) {
                                dt.row.add(addedItem).draw();
                            },
                            function (deletedItem) {
                                var rowIdx = dt.column(0).data().indexOf(deletedItem.Cd_Erro);
                                dt.row(rowIdx).remove().draw();
                            }
                        );

                        // Convert the data set into observable objects, and will also add the
                        // initial data to the table
                        ko.mapping.fromJS(
                            dataJson,
                            {
                                key: function (dataJson) {
                                    return ko.utils.unwrapObservable(dataJson.Cd_Erro);
                                },
                                create: function (options) {
                                    return new HealthCheck(options.data, dt);
                                }
                            },
                            st
                        );
                        //cria o grafico
                        AmCharts.makeChart("chart1", {
                            "type": "pie",
                            "titles": [{
                                "text": $("#dropDownListCategory option:selected").text(),
                                "size": 16
                            }],
                            "dataProvider": dataJson,
                            "valueField": "Qt_Ocorrencia",
                            "titleField": "Ds_Erro",
                            "labelRadius": 15,
                            "innerRadius": "40%",
                            "depth3D": 5,
                            "balloonText": "[[title]]<br><span style='font-size:14px'><b>[[Qt_Ocorrencia]]</b> ([[Pc_Ocorrencia_Total]]%)</span>",
                            "angle": 15,
                            "legend": { "position": "right" }
                        });
                        setInterval(function () { $('.amcharts-chart-div').children('a').remove(); }, 100);
                    }
                },
                error: function (xhr, status, p3, p4) {
                    $('#ModalLoadingHC').modal('toggle');
                    var err = "Error " + " " + status + " " + p3 + " " + p4;
                    if (xhr.responseText && xhr.responseText[0] == "{")
                        err = JSON.parse(xhr.responseText).Message;
                    console.log(err);
                }
            });
        } else {
            $("#ToDoListError").hide();
        }

    }

    self.GenerateTableView = function () { $("#ModalResultTable").modal(); }

    self.IntervalDataChart = ko.observable("");

    var dTable = $('#DtUltimoProcess').DataTable({
        "bLengthChange": false,
        "bPaginate": false,
        "bFilter": false,
        "bInfo": false,
        "columnDefs": [            
            { width: "10%", "targets": [0] },
            { width: "30%", "targets": [1] },
            { width: "30%", "targets": [2] },
            { className: "datatable-Pegasus-Col03", "targets": [3] },
            { className: "datatable-Pegasus-Col04", "targets": [4] }
        ],
        columns: [
            { data: 'Id_Processamento', title: "ID" },
            { data: 'Fl_Situacao', title: "Status" },
            { data: 'Dt_Fim_Processamento_Grid', title: "Último Processamento" },
            /* Download Excel */ {
                mRender: function (data, type, row) {
                    //return '<a data-id="' + row[0] + '" style="color: #fff; font-weight: bold;" >Gerar Excel</a>'
                   // return '<a data-id="' + row[0] + 'class="btn btnPrincipal" ><i class="fa fa-file-excel-o" style="color: #fff; font-weight: bold;"></i><span style="color: #fff; font-weight: bold;"> Gerar Excel </span></a>'
                    return '<button data-id="' + row[0] + '" class="btn btnPrincipal" onclick="javascript:void(0);" ><i class="fa fa-file-excel-o" style="color: #fff; font-weight: bold;"></i><span style="color: #fff; font-weight: bold;"> Gerar Excel </span></button>'
                    //class="fa fa-file-excel-o"
                }
            },
            /* Atualizar */ {
                mRender: function (data, type, row) {
                    //return '<a data-id="' + row[0] + '" style="color: #fff; font-weight: bold;">Atualizar</a>'
                    //return '<a data-id="' + row[0] + 'class="btn btnPrincipal" ><i class="fa fa-refresh" style="color: #fff; font-weight: bold;"></i><span style="color: #fff; font-weight: bold;"> Atualizar </span></a>'
                    return '<button data-id="' + row[0] + '" class="btn btn-blue" ><i class="fa fa-refresh" style="color: #fff; font-weight: bold;"></i><span style="color: #fff; font-weight: bold;"> Atualizar </span></button>'
                    //class="fa fa-refresh"
                }
            }
        ]
        //,
        //"language": {
        //    "url": "../../json/Portuguese-Brasil.json"
        // }
    });

    $('#DtUltimoProcess tbody').on('click', 'td', function () {
        //Se for Exportar Excel
        if (dTable.cell(this).index().column == 3) {
            var idProcessamento = parseInt(dTable.cell(dTable.cell(this).index().row, 0).data());

            var descStatus = dTable.cell(dTable.cell(this).index().row, 1).data();
            if (descStatus !== "Pendente" &&
                descStatus !== "Iniciado" &&
                idProcessamento > 0) {

                $("#ModalLoading").modal();
                //$("#ModalLoading").show();

                //#("#st-content-step10").show();
                $.ajax({
                    type: "POST",
                    url: util.mapUrl("/HealthCheck/GetValidImportPrice"),
                    data: JSON.stringify(idProcessamento),
                    contentType: "application/json",
                    success: function (results) {
                        $('#ModalLoading').modal('toggle');
                        //$("#ModalLoading").close();
                        if (results.hasErrors) {
                            util.ValidationResults.ShowErrors(results);
                        }
                        else {
                            if (results.Identifier) {
                                var identifier = results.Identifier;
                                var url = util.mapUrl("/HealthCheck/DownloadPrice/?identifier=" + identifier);
                                window.open(url, '_blank');
                            } else {
                                util.ShowWarning("Não foi possível gerar o excel!");
                            }
                        }
                    }
                });
            } else {
                utils.ShowWarning(descStatus + " Não há erros para exibir!");
            }
        }
        //Se for Atualizar a lista
        if (dTable.cell(this).index().column == 4) {
            var idProcessamento = dTable.cell(dTable.cell(this).index().row, 0).data();
            //location.reload();
            dTable.clear();
            dTable.draw();
            viewModel.GetListLastIntegrationProcesses();            
        }
    });

    // ImportPrice object
    var ImportPriceLastResult = function (data, dTable) {
        this.Id_Processamento = data.Id_Processamento;
        this.Fl_Situacao = ko.observable(data.Fl_Situacao);
        this.Dt_Fim_Processamento_Grid = ko.observable(data.Dt_Fim_Processamento_Grid);
        this.Ds_Parametro = ko.observable(data.Ds_Parametro);
        this.Fl_Tipo_Execucao = ko.observable(data.Fl_Tipo_Execucao);

        // Subscribe a listener to the observable properties for the table
        // and invalidate the DataTables row when they change so it will redraw
        var that = this;
        $.each(['Fl_Situacao', 'Dt_Fim_Processamento_Grid', 'Ds_Parametro', 'Fl_Tipo_Execucao'], function (i, prop) {
            that[prop].subscribe(function (val) {
                // Find the row in the DataTable and invalidate it, which will
                // cause DataTables to re-read the data
                var rowIdx = dTable.column(0).data().indexOf(that.Id_Processamento);
                dTable.row(rowIdx).invalidate();
            });
        });
    };
          
    //var self = this;

 
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

    self.ShowModal = function (id) {
        $(id).modal();
        $(this).find('input:text').val('');
        $('#st-content-Teste').children().hide();
        $('#st-content-step1').show();
    }

    self.ReportDateViewModel = new ReportDateViewModel(dashboard);

    self.UploadFile = function () {

        var files = $("#FileInput").get(0).files;
        if (files.length == 0) {
            util.ShowWarning('Selecione um Arquivo Excel!');
            return;
        }

        var data = new FormData();
        var fileSize = 0;
        var fileExtension = '';
        $.each($('#FileInput')[0].files, function (i, file) {
            data.append('file-' + i, file);
            fileSize = file.size;
            fileExtension = file.name.replace(/^.*\./, '');
        });

        //Valida tamanho do Arquivo
        if (fileSize > 5242880) {
            util.ShowWarning('Por favor, selecione um arquivo com até 5Mb!');
            return;
        }

        //Valida tamanho do Arquivo
        if (fileExtension != 'xls' && fileExtension != 'xlsx') {
            util.ShowWarning('Por favor, selecione um arquivo válido!');
            return;
        }

        $("#LoadingExcelFile").show();
        $("#LoadedExcel").hide();

        $.ajax({
            url: util.mapUrl("/HealthCheck/UploadStreamingFile"),
            data: data,
            cache: false,
            contentType: false,
            processData: false,
            method: 'POST',
            success: function (result) {
                $("#LoadingExcelFile").hide();
                $("#LoadedExcel").show();
            },
            error: function (xhr, status, p3, p4) {
                var err = "Error " + " " + status + " " + p3 + " " + p4;
                if (xhr.responseText && xhr.responseText[0] == "{")
                    err = JSON.parse(xhr.responseText).Message;
                console.log(err);
            }
        });
    }

    self.checkStepHS = function (divH, divS) {

        if (divH == '#st-content-step2') {

            var files = $("#FileInput").get(0).files;
            if (files.length == 0) {
                util.ShowWarning('Selecione um Arquivo Excel!');
                return;
            }

            var date = $("#dateRerenceFile").val();

            if (date == '') {
                util.ShowWarning('Selecione um Período de Referência!');
                return;
            }

            $("#LoadingDataStep1").show();

            $.ajax({
                type: "POST",
                url: util.mapUrl("/HealthCheck/CheckDE"),
                data: JSON.stringify(date),
                contentType: "application/json",
                success: function (results) {

                    $("#LoadingDataStep1").hide();

                    if (results.hasErrors) {
                        util.ValidationResults.ShowErrors(results);
                    } else {

                        $("#LoadedExcel").hide();
                        $("#LoadingExcelFile").hide();

                        $("#spnDtReference").text(date);
                        $('#spnFileName').text($('#FileInput').val().replace(/\\/g, '/').replace(/.*\//, ''))


                        if (results.hasDate == 'true') {
                            $(divH).hide();
                            $(divS).show();
                        } else {
                            $(divH).hide();
                            $('#st-content-step4').show();
                        }
                    }

                    return;
                }
            });

        } else if (divH == '#st-content-step4' && divS == '#st-content-step5') {

            $(divH).hide();
            $(divS).show();

            var date = $("#dateRerenceFile").val();

            $("#LoadingDataStep2").show();

            $.ajax({
                type: "POST",
                url: util.mapUrl("/HealthCheck/SaveDataPriceExcel"),
                data: JSON.stringify(date),
                contentType: "application/json",
                success: function (results) {
                    $("#LoadingDataStep2").hide();

                    if (results.hasErrors) {
                        util.ValidationResults.ShowErrors(results);
                    }
                    else {
                        $('#st-content-step5').hide();
                        $('#st-content-step9').show();

                        setTimeout(function () {
                            dTable.clear();
                            dTable.draw();
                            viewModel.GetListLastIntegrationProcesses();                            
                            $('#st-content-step1').show();
                            $('#ModalImportPrice').modal('toggle');
                            //location.reload();
                        }, 2500);

                       
                    }

                }
            });

        } else if (divH == '#st-content-step5' && divS == '#st-content-step4') {

            location.reload();

        } else if (divH == '#st-content-step7' && divS == '#st-content-step1') {

            location.reload();

        } else if (divH == '#st-content-step9' && divS == '#st-content-step7') {
            //$(divH).hide();
            //$(divS).show();
            $("#LoadingDataStep4").show();

            $.ajax({
                type: "GET",
                url: util.mapUrl("/HealthCheck/ValidDataPrice"),
                contentType: "application/json",
                success: function (results) {

                    $("#LoadingDataStep4").hide();

                    if (results.hasErrors) {
                        util.ValidationResults.ShowErrors(results);

                    }


                    return;
                }
            });
        } else if (divS == '#st-content-step7') {

            $(divH).hide();
            $(divS).show();

            $("#LoadingDataStep3").show();

            $.ajax({
                type: "GET",
                url: util.mapUrl("/HealthCheck/SaveDataPrice"),
                contentType: "application/json",
                success: function (results) {

                    $("#LoadingDataStep3").hide();

                    if (results.hasErrors) {
                        util.ValidationResults.ShowErrors(results);
                    }
                    else {

                        //var result = JSON.parse(results.result);

                        $("#SuccessAgendST").show();

                        //$("#SuccessLoadST").show();
                        //$("#spnQtRegistroLido").text(result.Qt_Registro_Lido);
                        //$("#spnQtRegistroGravado").text(result.Qt_Registro_Gravado);
                        //$("#spnQtRegistroRejeitado").text(result.Qt_Registro_Rejeitado);
                    }

                    return;
                }
            });

        } else {
            $(divH).hide();
            $(divS).show();
        }

    }

    self.IntervalDataChart = ko.observable("");


    self.GetListLastIntegrationProcesses = function () {
        $.ajax({
            url: util.mapUrl("/HealthCheck/GetLastIntegrationProcesses"),
            contentType: false,
            processData: false,
            method: 'GET',
            success: function (result) {
                //var processItem = JSON.parse(result.Data);
                var st = ko.mapping.fromJS([]);
                var dataJson = JSON.parse(result.Data);

                // Update the table when the `people` array has items added or removed
                st.subscribeArrayChanged(
                    function (addedItem) {
                        dTable.row.add(addedItem).draw();
                    },
                    function (deletedItem) {
                        var rowIdx = dTable.column(0).data().indexOf(deletedItem.Id_Processamento);
                        dTable.row(rowIdx).remove().draw();
                    }
                );

                // Convert the data set into observable objects, and will also add the
                // initial data to the table
                ko.mapping.fromJS(
                    dataJson,
                    {
                        key: function (dataJson) {
                            return ko.utils.unwrapObservable(dataJson.Id_Processamento);
                        },
                        create: function (options) {
                            return new ImportPriceLastResult(options.data, dTable);
                        }
                    },
                    st
                );
            },
            error: function (xhr, status, p3, p4) {
                var err = "Error " + " " + status + " " + p3 + " " + p4;
                if (xhr.responseText && xhr.responseText[0] == "{")
                    err = JSON.parse(xhr.responseText).Message;
                console.log(err);
            }
        });
    };
}
