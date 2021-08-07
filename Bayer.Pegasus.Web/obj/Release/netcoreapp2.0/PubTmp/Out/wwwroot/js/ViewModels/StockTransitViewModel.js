function StockTransitViewModel(dashboard) {
    var dTable = $('#DtUltimoProcess').DataTable({
        "bLengthChange": false,
        "bPaginate": false,
        "bFilter": false,
        "bInfo": false,
        "columnDefs": [
            { className: "datatable-Pegasus-Col03", "targets": [3] },
            { className: "datatable-Pegasus-Col04", "targets": [4] }
        ],
        columns: [
            { data: 'Id_Processamento', title: "ID" },
            { data: 'Fl_Situacao', title: "Status" },
            { data: 'Dt_Fim_Processamento_Grid', title: "Último Processamento" },
            /* Download Excel */            
            {
                mRender: function (data, type, row) {
                    return '<a data-id="' + row[0] + 'class="btn btnPrincipal" ><i class="fa fa-file-excel-o" style="color: #fff; font-weight: bold;"></i><span style="color: #fff; font-weight: bold;"> Gerar Excel </span></a>'
                    //class="fa fa-file-excel-o"
                }
            },
            /* Atualizar */ {
                mRender: function (data, type, row) {                    
                    return '<a data-id="' + row[0] + 'class="btn btnPrincipal" ><i class="fa fa-refresh" style="color: #fff; font-weight: bold;"></i><span style="color: #fff; font-weight: bold;"> Atualizar </span></a>'
                    //class="fa fa-refresh"
                }
            }
        ],
        "language": {
            "url": "../../json/Portuguese-Brasil.json"
        }
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
                    url: util.mapUrl("/StockTransit/GetValidStockTransit"),
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
                                var url = util.mapUrl("/StockTransit/Download/?identifier=" + identifier);
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
            location.reload();
        }
    });

    // StockTransit object
    var StockTransitLastResult = function (data, dTable) {
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


    //var dt = $('#dt_st_erroGrid').DataTable({
    //    columns: [
    //        { data: 'id', title: "Número Linha" },
    //        { data: 'NF', title: "Nota Fiscal" },
    //        { data: 'CD', title: "Centro de Distribuição" },
    //        { data: 'NameRecipient', title: "Nome Destinatário" },
    //        { data: 'CNPJRecipient', title: "Documento Destinatário" },
    //        { data: 'Ds_Erro', title: "Erro" }
    //    ],
    //    "language": {
    //        "url": "../../json/Portuguese-Brasil.json"
    //    }
    //});

    //// StockTransit object
    //var StockTransit = function (data, dt) {
    //    this.id = data.NumberLine;
    //    this.NF = ko.observable(data.NF);
    //    this.CD = ko.observable(data.CD);
    //    this.NameRecipient = ko.observable(data.NameRecipient);
    //    this.CNPJRecipient = ko.observable(data.CNPJRecipient);
    //    this.Ds_Erro = ko.observable(data.Ds_Erro);

    //    // Subscribe a listener to the observable properties for the table
    //    // and invalidate the DataTables row when they change so it will redraw
    //    var that = this;
    //    $.each(['NF', 'CD', 'NameRecipient', 'CNPJRecipient', 'Ds_Erro'], function (i, prop) {
    //        that[prop].subscribe(function (val) {
    //            // Find the row in the DataTable and invalidate it, which will
    //            // cause DataTables to re-read the data
    //            var rowIdx = dt.column(0).data().indexOf(that.id);
    //            dt.row(rowIdx).invalidate();
    //        });
    //    });
    //};

    var self = this;

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
            url: util.mapUrl("/StockTransit/UploadStreamingFile"),
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
                url: util.mapUrl("/StockTransit/CheckDE"),
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
                url: util.mapUrl("/StockTransit/SaveDataTransitExcel"),
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
                            location.reload();                           
                        }, 5000);

                       
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
                url: util.mapUrl("/StockTransit/ValidDataTransit"),
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
                url: util.mapUrl("/StockTransit/SaveDataTransit"),
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
            url: util.mapUrl("/StockTransit/GetLastIntegrationProcesses"),
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
                            return new StockTransitLastResult(options.data, dTable);
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
