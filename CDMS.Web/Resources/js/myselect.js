layui.define(['jquery', 'utils'], function (exports) {
    var $ = layui.jquery, table = layui.table, utils = layui.utils;

    function mySelect() {
        var that = this;

        var defaults = {
            elem: '',
            value: 'value',
            text: 'text',
            selected: '',
            append: true,
            change: false,
            data: [],
            callback: null,
            ajax: {
                url: '',
                data: {}
            }
        };

        that.set(defaults);
    }

    mySelect.prototype.set = function (o) {
        this.options = $.extend(this.options, o || {});
        return this;
    }

    mySelect.prototype.init = function () {
        var that = this;
        var options = that.options, ajax = options.ajax;
        var data = options.data;
        if (data && data.length < 1 && ajax.url) {
            utils.ajax(ajax.url, ajax.data, function (data) {
                var flag = data.Type <= 1;
                if (flag) {
                    that.render(data.Data);
                    if (options.callback) options.callback(data);
                }
            }, false);
        }
        else {
            that.render(data);
            if (options.callback) options.callback(data);
        }
        return that;
    }

    mySelect.prototype.render = function (data) {
        var that = this, options = that.options, selector = options.elem;

        var html = '';
        $.each(data, function (i, item) {
            var value = '', text = '';
            if (typeof (item) == 'object') {
                value = item[options.value];
                text = item[options.text];
            }
            else {
                value = item;
                text = item;
            }
            html += '<option value="' + value + '">' + text + '</option>';
        });

        var select = $(selector);

        if (options.append) select.append(html);
        else select.html(html);

        select.val(options.selected);
        if (options.change) {
            select.change(options.change);
        }
    }

    var select_out = {
        render: function (o) {
            var sel = new mySelect();
            return sel.set(o).init();
        }
    };

    exports('myselect', select_out);
});