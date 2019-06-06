/**
* @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
* For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    // config.language = 'fr';
    // config.uiColor = '#AADC6E';
    CKEDITOR.config.allowedContent = true;
    CKEDITOR.config.fillEmptyBlocks = false;
    CKEDITOR.config.basicEntities = false;
    CKEDITOR.dtd.$removeEmpty['i'] = false;
    CKEDITOR.dtd.$removeEmpty.i = 0;
    CKEDITOR.dtd.$removeEmpty['a'] = false;
    CKEDITOR.dtd.$removeEmpty.a = 0;
    CKEDITOR.dtd.a.div = 1;
    CKEDITOR.dtd.$removeEmpty['span'] = false;
    CKEDITOR.dtd.$removeEmpty.span = 0;
    CKEDITOR.dtd.$removeEmpty['div'] = true;
    CKEDITOR.dtd.$removeEmpty.div = 1;
    CKEDITOR.config.autoParagraph = false;
    CKEDITOR.config.enterMode = false;
    CKEDITOR.config.shiftEnterMode = false;
    CKEDITOR.config.removeButtons = 'Save,About';
    (function () {
        CKEDITOR.on('instanceReady', function (e) {
            var instance = e.editor;
            var rules = {
                indent: false,
                breakBeforeOpen: false,
                breakAfterOpen: false,
                breakBeforeClose: false,
                breakAfterClose: true
            }
            instance.dataProcessor.writer.setRules('p', rules);
            instance.dataProcessor.writer.setRules('div', rules);
        });
    })();
    (function () {
        CKEDITOR.on('instanceCreated', function (e) {
            CKEDITOR.dtd.$removeEmpty['span'] = false;
            CKEDITOR.dtd.$removeEmpty['a'] = false;
            CKEDITOR.dtd.$removeEmpty['i'] = false;
            CKEDITOR.dtd.$removeEmpty['div'] = true;
            CKEDITOR.dtd.$removeEmpty['p'] = true;
        });
    })();
    (function () {
        var isChrome = /Chrome/.test(navigator.userAgent) && /Google Inc/.test(navigator.vendor);
        if (isChrome) {
            CKEDITOR.on('instanceLoaded', function (e) {
                this.addCss('.cke_editable { line-height: inherit; }');
            });
        }
    })();
};