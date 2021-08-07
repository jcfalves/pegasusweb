

ko.tokenfield = {};
var engine = new Array();
var tokens = new Array();
var checkAvailable = true;
/**
 * Tokenfield custom binding
 */
ko.bindingHandlers.tokenField = {

	/**
     * ko binding init
     */
    init: function(element, valueAccessor, allBindingsAccessor, deprecated, bindingContext) {
        console.log('--INIT:' + element.id);
        
		var observable = valueAccessor() || { };
		//var bindings = new tokenFieldUtils().processBindings(allBindingsAccessor);

		/**
		 * Setup config for element in global namespace.
		 */
		ko.tokenfield[element.id] = {};
		ko.tokenfield[element.id].handlerEnabled = true;

		ko.tokenfield[element.id].bindings = {};

		ko.tokenfield[element.id].bindings['Remote']		= allBindingsAccessor().tokenFieldRemote;
		ko.tokenfield[element.id].bindings['Method']		= allBindingsAccessor().tokenFieldMethod;
		ko.tokenfield[element.id].bindings['Datatype']		= allBindingsAccessor().tokenFieldDatatype;
		ko.tokenfield[element.id].bindings['Query']			= allBindingsAccessor().tokenFieldQuery;
		ko.tokenfield[element.id].bindings['KeyIndex']		= allBindingsAccessor().tokenFieldKeyIndex;
		ko.tokenfield[element.id].bindings['KeyDisplay']	= allBindingsAccessor().tokenFieldKeyDisplay;
        ko.tokenfield[element.id].bindings['Delimiter']     = allBindingsAccessor().tokenFieldDelimiter;
        ko.tokenfield[element.id].bindings['CreateTokensOnBlur'] = allBindingsAccessor().tokenFieldCreateTokensOnBlur;
        ko.tokenfield[element.id].bindings['ParentValue']   = allBindingsAccessor().ParentValue;
        ko.tokenfield[element.id].bindings['Parent']        = allBindingsAccessor().Parent;

		if (ko.tokenfield[element.id].bindings['Remote']	=== undefined) throw('Tokenfield remote server required.');
		if (ko.tokenfield[element.id].bindings['Method']	=== undefined) ko.tokenfield[element.id].bindings['Method']		= 'POST';
		if (ko.tokenfield[element.id].bindings['Datatype']	=== undefined) ko.tokenfield[element.id].bindings['Datatype']	= 'json';
        if (ko.tokenfield[element.id].bindings['Query'] === undefined) ko.tokenfield[element.id].bindings['Query'] = 'search';
		if (ko.tokenfield[element.id].bindings['KeyIndex']	=== undefined) ko.tokenfield[element.id].bindings['KeyIndex']	= 'value';
		if (ko.tokenfield[element.id].bindings['KeyDisplay']=== undefined) ko.tokenfield[element.id].bindings['KeyDisplay']	= 'label';
        if (ko.tokenfield[element.id].bindings['Delimiter'] === undefined) ko.tokenfield[element.id].bindings['Delimiter'] = ',';
        if (ko.tokenfield[element.id].bindings['CreateTokensOnBlur'] === undefined) ko.tokenfield[element.id].bindings['CreateTokensOnBlur'] = false;
        if (ko.tokenfield[element.id].bindings['ParentValue'] === undefined) ko.tokenfield[element.id].bindings['ParentValue'] = '';
        if (ko.tokenfield[element.id].bindings['Parent'] === undefined) ko.tokenfield[element.id].bindings['Parent'] = '';

		/**
		 * Destroy tokenfield
		 *
		 * Handle disposal (if KO removes by the template binding)
		 */
		ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
			console.log('Destroy tokenfield');
			$(element).tokenfield('destroy');
			// TODO: Destroy tokenFieldModel.
		});

		/**
		 * Create token
		 *
		 * Push into observableArray().items
		 */
		 /*
		ko.utils.registerEventHandler(element, 'tokenfield:createtoken', function (e) {
			console.log('tokenfield:createtoken:'+this.id);
			//e.attrs.tokenfieldFromUi = true;
			console.log(JSON.stringify(e.attrs));
		});
		*/

		
		ko.utils.registerEventHandler(element, 'tokenfield:createdtoken', function (e) {
			console.log('tokenfield:createdtoken:'+this.id);
			console.log(JSON.stringify(e.attrs));

		    // Detect private token created.
			if (e.attrs[ko.tokenfield[element.id].bindings['KeyDisplay']].indexOf("_") === 0) {
				console.log('tt-private');
				$(e.relatedTarget).addClass('tt-private');
			}

			// Allow `update` to temporarily disable pushing back when this event fires.
			if (ko.tokenfield[element.id].handlerEnabled === true) observable.push(e.attrs);

		});

		/**
		 * Remove token
		 *
		 * Remove from observableArray().items
		 */
		ko.utils.registerEventHandler(element, 'tokenfield:removedtoken', function (e) {
			console.log('tokenfield:removedtoken:'+e.value);
			console.log(JSON.stringify(e.attrs));

			var peeked = observable.peek();
			var item;
			// Find item using tokenfield default values, other values are not in tokenfield meta-data.
            ko.utils.arrayForEach(peeked, function (x) {

                if (ko.unwrap(x.value) === e.attrs.value) {
					item = x;
				} else if (ko.unwrap(x.label) === e.attrs.label && ko.unwrap(x.value) === e.attrs.value) {
					item = x;
				}
			});

			observable.remove(item);
		});

		/**
		 * Typeahead only, no tokenfield
		 *
		 * @todo: If delimiter isn't set only Typeahead is needed.
		  */
		/*
		ko.utils.registerEventHandler(element, 'typeahead:selected typeahead:autocompleted', function (e) {
			console.log('typeahead:selected typeahead:autocompleted');
			console.log(e);
			valueAccessor().value = e.attrs.value;	
		});
		*/
        

		/**
		 * Initalise tokenfield.
		 */

        function customTokenizer(datum) {
            var value = Bloodhound.tokenizers.whitespace(datum.value);
            var label = Bloodhound.tokenizers.whitespace(datum.label);
            var cnpj = Bloodhound.tokenizers.whitespace(datum.cnpj);

            return value.concat(label).concat(cnpj);
        }

        $.ajax({
            url: ko.tokenfield[element.id].bindings['Remote'],
            type: ko.tokenfield[element.id].bindings['Method'],
            contentType: 'application/json',
            dataType: ko.tokenfield[element.id].bindings['Datatype'],
            success: function (data) {
                tokens[element.id] = data;
            },
            error: function () {
                response([]);
            },
            complete: function () {
                engine[element.id] = new Bloodhound({
                    local: tokens[element.id],
                    identify: function (obj) { return obj.value; },
                    datumTokenizer: function (d) {
                        return customTokenizer(d);
                    },
                    queryTokenizer: Bloodhound.tokenizers.whitespace
                });

                $('#' + element.id).val('');

                engine[element.id].initialize();

                $(element).tokenfield({
                    delimiter: ko.tokenfield[element.id].bindings['Delimiter'],
                    createTokensOnBlur: true,
                    allowEditing: false,
                    typeahead: [{
                        minLength: 3
                    }, {
                        name: element.id,
                        displayKey: ko.tokenfield[element.id].bindings['KeyDisplay'],
                        source: engine[element.id].ttAdapter()
                    }]
                });

                $(element).on('tokenfield:createtoken', function (event) {
                    var available_tokens = tokens[element.id]
                    var exists = false;
                    $.each(available_tokens, function (index, token) {
                        if (token.value == event.attrs.value)
                            exists = true;
                    });
                    if (exists === false && checkAvailable)
                        event.preventDefault();
                });

                $(element).parent().on('keyup', function (event) {
                    if (event.which == 9) {
                        $(".tt-suggestion:first-child", $(this)).trigger('click');
                        return;
                    }

                    var searchVal = $(this).find('#' + element.id + '-tokenfield').val();
                    var jsonParents = ko.tokenfield[element.id].bindings['ParentValue'];
                    if(searchVal.length < 3)
                        return;

                    $.ajax({
                        url: ko.tokenfield[element.id].bindings['Remote'],
                        type: ko.tokenfield[element.id].bindings['Method'],
                        contentType: 'application/json',
                        dataType: ko.tokenfield[element.id].bindings['Datatype'],
                        data: ko.toJSON({
                            search: searchVal,
                            parents: jsonParents
                        }),
                        success: function (data) {
                            tokens[element.id] = data;
                        },
                        error: function () {
                            response([]);
                        },
                        complete: function () {
                            engine[element.id].clear();
                            engine[element.id].local = tokens[element.id];
                            var promise = engine[element.id].initialize(true);
                            promise.done(function () {
                                if ($('.tt-dataset-' + element.id).children().length == 0) {
                                    var e = jQuery.Event("keydown");
                                    e.which = 40;
                                    $('#' + element.id + '-tokenfield').trigger(e);
                                }
                            });
                        }
                    });
                });
            }
        });

		//return { controlsDescendantBindings: true };

    },

    /**
     * ko binding update
     */
    update: function (element, valueAccessor, allBindingsAccessor, deprecated, bindingContext) {
        checkAvailable = false;
        ko.unwrap(allBindingsAccessor());
        console.log('--UPDATE:'+element.id);
        var observable = valueAccessor() || { };
        var peeked = ko.unwrap(observable.peek());
        ko.tokenfield[element.id].handlerEnabled = false;
        if (observable().length > 0) {
            $(element).tokenfield('setTokens', ko.toJS(peeked));
        } else {
            $(element).tokenfield('setTokens', []);
        }
        checkAvailable = true;
        /*
		$(element).tokenfield('setTokens',[]);
        if (peeked.length > 0) {
    		console.log('Create tokens from array');
    		$.each(peeked, function(index, value) {
    			console.log('item:'+JSON.stringify(value));
    			value.foobar = true;
    			$(element).tokenfield('createToken', value);
    		});
    	}
        */
        ko.tokenfield[element.id].handlerEnabled = true;
    }

};
