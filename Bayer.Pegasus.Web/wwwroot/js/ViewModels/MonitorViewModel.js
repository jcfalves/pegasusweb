function MonitorViewModel(dashboard) {

    $("#numCNPJDistr").mask("99.999.999/9999-99");
    $('#integracaoApi').hide();
    $('#integracaoFTP').hide();
    $('#integracaoAPIEstoque').hide();

    $('.container div').delay(15000).fadeOut("slow");
    var self = this;

    var radioValue = '';
    var fileName = '';

    self.Partners = ko.observableArray([]);

    self.Integracoes = ko.observableArray([]);

    self.IntegracoesE = ko.observableArray([]);

    self.StepCaptacao = ko.observableArray([]);

    self.StepValidacao = ko.observableArray([]);

    //novaVariavel para nova Tela
    self.IntegracoesLogs = ko.observableArray([]);
    self.IntegracoesLgosE = ko.observableArray([]);

    self.StepODS = ko.observableArray([]);

    self.StepDW = ko.observableArray([]);

    self.StepDMI = ko.observableArray([]);

    self.StepPreparacao = ko.observableArray([]);

    self.StepEnvio = ko.observableArray([]);

    self.ShowModal = function (id) {
        $(id).modal();
    }

    var $radios = $('input[name=tipo-FTP]').change(function () {
        radioValue = $radios.filter(':checked').val();
        if (radioValue == 1) {
            $('#integracaoUpload').hide();
        }
        else if (radioValue == 2) {
            $('#integracaoUpload').show();
        }
    });

    self.ShowLogProcessamento = function (id, fase) {

        var params = JSON.stringify({ ID: id, FASE: fase });

        $.ajax({
            type: "POST",
            url: util.mapUrl("/Monitor/GetLogProcessamento"),
            data: params,
            contentType: "application/json",
            success: function (result) {
                var log = JSON.parse(result.log);
                var msglog = "";
                if (log.length > 0) {
                    jQuery.each(log, function (index, item) {
                        if (item.Fl_Tipo_Log != 'D' && item.Fl_Tipo_Log != '') {
                            if (msglog) {
                                msglog = msglog + " \n <br>" + item.Ds_Log_Processamento;
                            } else {
                                msglog = item.Ds_Log_Processamento;
                            }
                        }

                    });
                    util.ShowSuccessMessage(msglog);
                }
                else
                    util.ShowWarning('Não foi encontrado nenhum registro de log');
            },
            error: function (xhr, status, p3, p4) {
                var err = "Error " + " " + status + " " + p3 + " " + p4;
                if (xhr.responseText && xhr.responseText[0] == "{")
                    err = JSON.parse(xhr.responseText).Message;
                console.log(err);
            }
        });

    }

    self.dropDownListIntegracao = ko.observableArray();
    self.dropDownListIntegracaoFiltro = ko.observableArray();

    self.GetListIntegracao = function () {
        $.ajax({
            url: util.mapUrl("/Monitor/GetIntegrationProcessesManually"),
            contentType: false,
            processData: false,
            method: 'GET',
            success: function (result) {

                var steps = JSON.parse(result.result);
                $.each(steps, function (index, monitor) {
                    if (monitor.Fl_Ativo == "S") {
                        //if (monitor.Id_Origem_Carga == 1 || monitor.Id_Origem_Carga == 3) {
                        var item = {
                            name: monitor.Nm_Integracao,
                            value: monitor.Id_Origem_Carga + "|" + monitor.Cd_Integracao,
                        };
                        self.dropDownListIntegracao.push(item);
                    }
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

    self.GetListIntegracaoFiltro = function () {
        $.ajax({
            url: util.mapUrl("/Monitor/GetIntegrationProcesses"),
            contentType: false,
            processData: false,
            method: 'GET',
            success: function (result) {

                var steps = JSON.parse(result.result);
                $.each(steps, function (index, integracao) {
                    if (integracao.Fl_Ativo == "S") {
                        var item = {
                            name: integracao.Nm_Integracao,
                            value: integracao.Fl_Fluxo + "|" + integracao.Cd_Integracao,
                        };
                        self.dropDownListIntegracaoFiltro.push(item);
                    }                    
                });

                //self.dropDownListIntegracaoFiltro.push({
                //    name: 'Todos',
                //    value: 'T'
                //}); 

            },
            error: function (xhr, status, p3, p4) {
                var err = "Error " + " " + status + " " + p3 + " " + p4;
                if (xhr.responseText && xhr.responseText[0] == "{")
                    err = JSON.parse(xhr.responseText).Message;
                console.log(err);
            }
        });
    };

    self.permissionChanged = function (l) {

        $('#integracaoApi').hide();
        $('#integracaoFTP').hide();
        $('#integracaoAPIEstoque').hide();

        var integracao = $("#selIntegracao option:selected").val().split('|');
        var idOrigemCarga = integracao[0];
        var idIntegracao = integracao[1];
        if (integracao == "") {
            $('#integracaoApi').hide();
            $('#integracaoFTP').hide();
            $('#integracaoAPIEstoque').hide();
        }
        //var integracao = $("#selIntegracao option:selected").val();            
        if (idOrigemCarga == 1) {
            $('#integracaoApi').hide();
            $('#integracaoFTP').hide();
            $('#integracaoAPIEstoque').hide();
        }
        if (idOrigemCarga == 3 && idIntegracao != 5) {
            $('#integracaoFTP').hide();
            $('#integracaoApi').show();
            $('#integracaoAPIEstoque').hide();
        }
        if (idIntegracao == 5) {
            $('#integracaoAPIEstoque').show();
        }
    }

    self.DeleteFilterStep = function () {
        $('#integracaoMonitoramentoI').hide();
        $('#integracaoMonitoramentoE').hide();
        $('#FiltroAplicado').text("");
        $('.container div').show();

        //$('#ModalFiltroMonitor').modal('toggle');        
        $(".modal-body input").val("");
        $(".modal-body select").val("");        
        $(".modal-body input").prop("checked", false);

        self.UpdateStep();
    }        

    self.ChangeUpdateStep = function () {
        $('#integracaoMonitoramentoI').hide();
        $('#integracaoMonitoramentoE').hide();
        $('.container div').show();

        if ($('#selIntegracaoFiltro option:selected').attr('value')) {
            var integracao = $("#selIntegracaoFiltro option:selected").val().split('|');
            var integracao_name = $("#selIntegracaoFiltro option:selected").text();                        
        } 

        if (integracao && integracao_name) {
            self.FilterProcessos();
        } else {
            self.UpdateStep();
        }


    }

    self.UpdateStep = function () {       
            $.ajax({
                url: util.mapUrl("/Monitor/UpdateStep"),
                contentType: false,
                processData: false,
                method: 'GET',
                success: function (result) {

                    self.Integracoes.removeAll();
                    self.IntegracoesE.removeAll();
                    self.StepCaptacao.removeAll();
                    self.StepValidacao.removeAll();
                    self.StepODS.removeAll();
                    self.StepDW.removeAll();
                    self.StepDMI.removeAll();
                    self.StepPreparacao.removeAll();
                    self.StepEnvio.removeAll();
                    self.IntegracoesLogs.removeAll();
                    self.IntegracoesLgosE.removeAll();

                    var integrationList = JSON.parse(result.integrationList);
                    $('#LastProc').text("Última Atualização da Tela: " + JSON.parse(result.LastProc));

                    if (integrationList.length > 0) {
                        $.each(integrationList, function (index, integration) {

                            if (integration.Fl_Fluxo == 'I') {
                                $('#integracaoMonitoramentoI').show();
                                self.Integracoes.push({ Name: integration.Nm_Integracao });

                                self.IntegracoesLogs.push({ IdProc: integration.Steps[0].Id_Processamento, name: integration.Steps[0].Nm_Integracao, cap: returnFase(integration.Steps[0].CAPTACAO), pre: returnFase(integration.Steps[0].VALIDACAO), ODS: returnFase(integration.Steps[0].ODS), DW: returnFase(integration.Steps[0].DW), DMI: returnFase(integration.Steps[0].DMI) });
                                $.each(integration.Steps, function (index, step) {
                                    var cssClass;

                                    cssClass = returnFases(step, 'CAPTACAO');
                                    self.StepCaptacao.push({ CSSResult: cssClass, IdProc: step.Id_Processamento });

                                    cssClass = returnFases(step, 'VALIDACAO');
                                    self.StepValidacao.push({ CSSResult: cssClass, IdProc: step.Id_Processamento });

                                    cssClass = returnFases(step, 'ODS');
                                    self.StepODS.push({ CSSResult: cssClass, IdProc: step.Id_Processamento });

                                    cssClass = returnFases(step, 'DW');
                                    self.StepDW.push({ CSSResult: cssClass, IdProc: step.Id_Processamento });

                                    cssClass = returnFases(step, 'DMI');
                                    self.StepDMI.push({ CSSResult: cssClass, IdProc: step.Id_Processamento });
                                });
                            }

                            if (integration.Fl_Fluxo == 'E') {
                                $('#integracaoMonitoramentoE').show();
                                self.IntegracoesE.push({ Name: integration.Nm_Integracao });

                                self.IntegracoesLgosE.push({ name: integration.Nm_Integracao, IdProc: integration.Steps[0].Id_Processamento, preparacao: returnFase(integration.Steps[0].PREPARACAO), envio: returnFase(integration.Steps[0].ENVIO) });

                                $.each(integration.Steps, function (indexE, stepE) {
                                    var cssClass;

                                    cssClass = returnFases(stepE, 'PREPARACAO');
                                    self.StepPreparacao.push({ CSSResult: cssClass, IdProc: stepE.Id_Processamento });

                                    cssClass = returnFases(stepE, 'ENVIO');
                                    self.StepEnvio.push({ CSSResult: cssClass, IdProc: stepE.Id_Processamento });

                                });
                            }
                        });
                        $('.container div').hide();
                    }
                    else {
                        $('#integracaoMonitoramentoI').hide();
                        $('#integracaoMonitoramentoE').hide();
                        util.ShowWarning('Não foram encontrados processos para as integrações!');
                        $('.container div').delay(10000).fadeOut("slow");
                    }
                },
                error: function (xhr, status, p3, p4) {
                    var err = "Error " + " " + status + " " + p3 + " " + p4;
                    if (xhr.responseText && xhr.responseText[0] == "{")
                        err = JSON.parse(xhr.responseText).Message;
                    console.log(err);
                }
            });
    }

    self.FilterProcessos = function () {

        $('#integracaoMonitoramentoI').hide();
        $('#integracaoMonitoramentoE').hide();
        var integracao = $("#selIntegracaoFiltro option:selected").val().split('|');
        var integracao_name = $("#selIntegracaoFiltro option:selected").text();

        if (integracao == '') {
            util.ShowWarning('Selecione uma Integração!');
            return;
        }

        var flow = integracao[0];
        var code = integracao[1];
        var Exce_A = false;
        var Exce_M = false;

        if ($('#chkManual').is(':checked'))
            Exce_M = true;

        if ($('#chkAutomatico').is(':checked'))
            Exce_A = true;

        if ($("#selSituacao option:selected").val() == 'N') {
            util.ShowWarning('Selecione uma situação final!');
            return;
        }

        if ($("#selSituacao option:selected").val() == 'N') {
            util.ShowWarning('Selecione um tipo de execução!');
            return;
        }

        if ($("#dateFilterIni").val() == '') {
            util.ShowWarning('Selecione um Período de Referência Inicial!');
            return;
        }

        if ($("#dateFilterEnd").val() == '') {
            util.ShowWarning('Selecione um Período de Referência Final!');
            return;
        }

        if (Exce_A == false && Exce_M == false) {
            util.ShowWarning('Selecione Manual ou Automático!');
            return;
        }

        var params = JSON.stringify({
            PeriodIni: $("#dateFilterIni").val(), PeriodEnd: $("#dateFilterEnd").val(), Fl_Tipo_Execucao_M: Exce_M,
            Fl_Tipo_Execucao_A: Exce_A, Flow: flow, Cd_Integracao: code, Fl_Situacao: $("#selSituacao option:selected").val()
        });

        var filtrosAplicado = "Integração: " + code + "; Período: " + $("#dateFilterIni").val() + " Até: " + $("#dateFilterEnd").val() + "; Automático: " + (Exce_A == true ? "S" : "N") + "; Manual: " + (Exce_M == true ? "S" : "N") + "; Situação Final: " + $("#selSituacao option:selected").text();


        $.ajax({
            type: "POST",
            url: util.mapUrl("/Monitor/UpdateStepFilter"),
            data: params,
            contentType: "application/json",
            success: function (result) {

                if (result.hasErrors) {
                    util.ValidationResults.ShowErrors(result);
                } else {
                    var integrationList = JSON.parse(result.integrationList);
                    $('#LastProc').text("Última Atualização da Tela: " + JSON.parse(result.LastProc));
                    $('#FiltroAplicado').text("Filtro Aplicado: " + filtrosAplicado);
                                       
                    if (integrationList[0].Steps.length == 0) {
                        $('#integracaoMonitoramentoI').hide();
                        $('#integracaoMonitoramentoE').hide();
                        util.ShowWarning('Não foi encontrado nenhum resultado para esse filtro!');
                        return;
                    }

                    self.Integracoes.removeAll();
                    self.IntegracoesE.removeAll();
                    self.StepCaptacao.removeAll();
                    self.StepValidacao.removeAll();
                    self.StepODS.removeAll();
                    self.StepDW.removeAll();
                    self.StepDMI.removeAll();
                    self.StepPreparacao.removeAll();
                    self.StepEnvio.removeAll();
                    self.IntegracoesLogs.removeAll();
                    self.IntegracoesLgosE.removeAll();

                    $.each(integrationList, function (indexL, integration) {

                        if (integration.Fl_Fluxo == 'I') {
                            $('#integracaoMonitoramentoI').show();
                            self.IntegracoesLogs.push({ IdProc: integration.Steps[0].Id_Processamento, name: integration.Steps[0].Nm_Integracao, cap: returnFase(integration.Steps[0].CAPTACAO), pre: returnFase(integration.Steps[0].VALIDACAO), ODS: returnFase(integration.Steps[0].ODS), DW: returnFase(integration.Steps[0].DW), DMI: returnFase(integration.Steps[0].DMI) });
                            $.each(integration.Steps, function (indexSI, stepI) {
                                var cssClass;

                                self.Integracoes.push({ Name: integracao_name });

                                cssClass = returnFases(stepI, 'CAPTACAO');
                                self.StepCaptacao.push({ CSSResult: cssClass, IdProc: stepI.Id_Processamento });

                                cssClass = returnFases(stepI, 'VALIDACAO');
                                self.StepValidacao.push({ CSSResult: cssClass, IdProc: stepI.Id_Processamento });

                                cssClass = returnFases(stepI, 'ODS');
                                self.StepODS.push({ CSSResult: cssClass, IdProc: stepI.Id_Processamento });

                                cssClass = returnFases(stepI, 'DW');
                                self.StepDW.push({ CSSResult: cssClass, IdProc: stepI.Id_Processamento });

                                cssClass = returnFases(stepI, 'DMI');
                                self.StepDMI.push({ CSSResult: cssClass, IdProc: stepI.Id_Processamento });
                            });
                        }

                        if (integration.Fl_Fluxo == 'E') {
                            $('#integracaoMonitoramentoE').show();
                            self.IntegracoesLgosE.push({ name: integration.Nm_Integracao, IdProc: integration.Steps[0].Id_Processamento, preparacao: returnFase(integration.Steps[0].PREPARACAO), envio: returnFase(integration.Steps[0].ENVIO) });
                            $.each(integration.Steps, function (indexSE, stepE) {
                                var cssClass;

                                self.IntegracoesE.push({ Name: integracao_name });

                                cssClass = returnFases(stepE, 'PREPARACAO');
                                self.StepPreparacao.push({ CSSResult: cssClass, IdProc: stepE.Id_Processamento });

                                cssClass = returnFases(stepE, 'ENVIO');
                                self.StepEnvio.push({ CSSResult: cssClass, IdProc: stepE.Id_Processamento });

                            });
                        }

                    });                    
                    $('#ModalFiltroMonitor').modal('hide');
                    $('.container div').hide();                    
                }


            },
            error: function (xhr, status, p3, p4) {
                var err = "Error " + " " + status + " " + p3 + " " + p4;
                if (xhr.responseText && xhr.responseText[0] == "{")
                    err = JSON.parse(xhr.responseText).Message;
                console.log(err);
            }
        });
    }

    self.SaveProcessos = function () {
        var integracao = $("#selIntegracao option:selected").val().split('|');
        var idOrigemCarga = "";

        if (integracao != '') {
            idOrigemCarga = integracao[0];
            codIntegracao = integracao[1];

            if (codIntegracao == 6) {
                var vmObj = ko.toJS(self);
                //console.debug(vmObj);
                if (vmObj.Partners.length > 0) {
                    var numCnpjDistr = vmObj.Partners[0].cnpj;
                } else {
                    var numCnpjDistr = '';
                }
                var dataInicio = $("#dateMonitorIni").val();
                var dataFim = $("#dateMonitorEnd").val()
                //var numCnpjDistr = $("#numCNPJDistr").val().replace(/\D/g, '');
                var numNotaFiscal = $("#numNotaFiscal").val();
                if (codIntegracao != 5) {
                    if (!codIntegracao) {
                        util.ShowWarning('Escolha uma opção de execução! ');
                        return;
                    }

                    if (numCnpjDistr == '' && !numCnpjDistr) {
                        util.ShowWarning('Digite o número do CNPJ!');
                        return;
                    }

                    if (numCnpjDistr.length < 14) {
                        util.ShowWarning('CNPJ inválido!');
                        return;

                    }
                    //if (dataInicio > dataFim) {
                    //    util.ShowWarning('Período invalido! ');
                    //    return;
                    //}
                    if (!dataInicio && !dataFim && !numCnpjDistr && !numNotaFiscal) {
                        util.ShowWarning('Necessário selecionar um filtro para Processamento Manual API! ');
                        return;
                    }
                }
                /*Montagem do Parametro*/
                var params = JSON.stringify({
                    PeriodIni: dataInicio,
                    PeriodEnd: dataFim,
                    Cd_Integracao: codIntegracao,
                    Id_Origem_Carga: idOrigemCarga,
                    NumCnpjDistr: numCnpjDistr,
                    NumNotaFiscal: numNotaFiscal
                });
                $.ajax({
                    type: "POST",
                    url: util.mapUrl("/Monitor/SaveProcessesApi"),
                    data: params,
                    contentType: "application/json",
                    success: function (results) {

                        if (results.hasErrors) {
                            util.ValidationResults.ShowErrors(results);
                        } else {
                            $("#dateMonitorIni").val("");
                            $("#dateMonitorEnd").val("");
                            //$("#numCNPJDistr").val("");
                            $('#ModalProcessamento').modal('hide');
                            $('.token').find('.close').click();
                            $("#numNotaFiscal").val("");
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
            }
            else if (codIntegracao == 5) {
                var params = JSON.stringify({
                    Id_Origem_Carga: idOrigemCarga,
                    Cd_Integracao: codIntegracao,
                    PeriodIni: $("#MonthDate").val()
                });
                $.ajax({
                    type: "POST",
                    url: util.mapUrl("/Monitor/SaveProcessesApi"),
                    data: params,
                    contentType: "application/json",
                    success: function (results) {

                        if (results.hasErrors) {
                            util.ValidationResults.ShowErrors(results);
                        } else {
                            $("#LoadingFile").hide();
                            $("#LoadedFile").hide();
                            $("#BrowseInputModal").val("");

                            $('#ModalProcessamento').modal('hide');
                            util.ShowSuccessMessage('Excução programada com sucesso!');
                        }

                    },
                    error: function (xhr, status, p3, p4) {
                        var err = "Error " + " " + status + " " + p3 + " " + p4;
                        if (xhr.responseText && xhr.responseText[0] == "{")
                            err = JSON.parse(xhr.responseText).Message;
                        console.log(err);
                    }
                });
            }
            else {
                //if (idOrigemCarga == 1 || idOrigemCarga == 4 || idOrigemCarga == 6) {
                /*Montagem do Parametro*/
                var params = JSON.stringify({
                    Cd_Integracao: codIntegracao
                });

                $.ajax({
                    type: "POST",
                    url: util.mapUrl("/Monitor/SaveProcessesFTP"),
                    data: params,
                    contentType: "application/json",
                    success: function (results) {

                        if (results.hasErrors) {
                            util.ValidationResults.ShowErrors(results);
                        } else {

                            $("#LoadingFile").hide();
                            $("#LoadedFile").hide();
                            $("#BrowseInputModal").val("");

                            $('#ModalProcessamento').modal('hide');
                            util.ShowSuccessMessage('Excução programada com sucesso!');
                        }

                    },
                    error: function (xhr, status, p3, p4) {
                        var err = "Error " + " " + status + " " + p3 + " " + p4;
                        if (xhr.responseText && xhr.responseText[0] == "{")
                            err = JSON.parse(xhr.responseText).Message;
                        console.log(err);
                    }
                });

            }
            /*
            if (idOrigemCarga == 1) {

                if (radioValue == '') {
                    util.ShowWarning('Escolha uma opção de execução! ');
                    return;
                }

                if (radioValue == '2') {

                    var files = $("#FileInputModal").get(0).files;
                    if (files.length == 0) {
                        util.ShowWarning('Selecione um Arquivo!');
                        return;
                    }
                }

                //Montagem do Parametro
                var params = JSON.stringify({
                    option: radioValue,
                    Cd_Integracao: codIntegracao,
                    fileName: fileName
                });
            
                $.ajax({
                    type: "POST",
                    url: util.mapUrl("/Monitor/SaveProcessesFTP"),
                    data: params,
                    contentType: "application/json",
                    success: function (results) {

                        if (results.hasErrors) {
                            util.ValidationResults.ShowErrors(results);
                        } else {

                            $("#LoadingFile").hide();
                            $("#LoadedFile").hide();
                            $("#BrowseInputModal").val("");
                            
                            $('#ModalProcessamento').modal('hide');
                            util.ShowSuccessMessage('Excução programada com sucesso!');
                        }

                    },
                    error: function (xhr, status, p3, p4) {
                        var err = "Error " + " " + status + " " + p3 + " " + p4;
                        if (xhr.responseText && xhr.responseText[0] == "{")
                            err = JSON.parse(xhr.responseText).Message;
                        console.log(err);
                    }
                });
            }   
            */
        }
        else {
            util.ShowWarning('Necessário selecionar um filtro para Processamento Manual! ');
            return;
        }
    }

    self.UploadFile = function () {

        var files = $("#FileInputModal").get(0).files;
        if (files.length == 0) {
            util.ShowWarning('Selecione um Arquivo!');
            return;
        }

        var data = new FormData();

        $.each($('#FileInputModal')[0].files, function (i, file) {
            data.append('file-' + i, file);
            fileSize = file.size;
            fileName = file.name;
        });

        $("#LoadingFile").show();
        $("#LoadedFile").hide();

        $.ajax({
            url: util.mapUrl("/Monitor/UploadStreamingFile"),
            headers: { "X-FileName-Header": fileName },
            data: data,
            cache: false,
            contentType: false,
            processData: false,
            method: 'POST',
            success: function (result) {

                $("#LoadedFile").show();
                $("#LoadingFile").hide();

                if (results.hasErrors) {
                    util.ValidationResults.ShowErrors(results);
                    $("#LoadedFile").hide();
                    $("#LoadingFile").hide();
                }

            },
            error: function (xhr, status, p3, p4) {
                var err = "Error " + " " + status + " " + p3 + " " + p4;
                if (xhr.responseText && xhr.responseText[0] == "{")
                    err = JSON.parse(xhr.responseText).Message;
                console.log(err);
            }
        });
    }

    var returnFase = function (statusCode) {
        var cssClass;
        switch (parseInt(statusCode)) {
            case 1:
                cssClass = 'glyphicon-dot-in-process';
                break;
            case 2:
                cssClass = 'glyphicon-dot-sucess';
                break;
            case 3:
                cssClass = 'glyphicon-dot-alert';
                break;
            case 4:
                cssClass = 'glyphicon-dot-error';
                break;
            default:
                cssClass = 'glyphicon-dot-none';
        }
        return cssClass;
    }

    var returnFases = function (object, fase) {
        var cssClass;
        var statusCode = -1;
        if (fase == 'CAPTACAO')
            statusCode = parseInt(object.CAPTACAO);
        else if (fase == 'VALIDACAO')
            statusCode = parseInt(object.VALIDACAO);
        else if (fase == 'ODS')
            statusCode = parseInt(object.ODS);
        else if (fase == 'DW')
            statusCode = parseInt(object.DW);
        else if (fase == 'DMI')
            statusCode = parseInt(object.DMI);
        else if (fase == 'PREPARACAO')
            statusCode = parseInt(object.PREPARACAO);
        else if (fase == 'ENVIO')
            statusCode = parseInt(object.ENVIO);
        else
            statusCode = -1;

        switch (statusCode) {
            case 1:
                cssClass = 'row glyphicon-dot-in-process';
                break;
            case 2:
                cssClass = 'row glyphicon-dot-sucess';
                break;
            case 3:
                cssClass = 'row glyphicon-dot-alert';
                break;
            case 4:
                cssClass = 'row glyphicon-dot-error';
                break;
            default:
                cssClass = 'row glyphicon-dot-none';
        }


        return cssClass;
    }
}
