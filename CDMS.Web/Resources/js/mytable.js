
layui.define(['jquery', 'table'], function (exports) {
    var $ = layui.jquery, table = layui.table;

    function myTable() {
        var tableId = 'myTable';
        var defaults = {
            id: tableId,
            elem: '#' + tableId,
            url: '',
            method: 'post',
            tr_click_checkable: true,
            page: true,
            limit: 10,
            limits: [10, 20, 30]
        };
        this.set(defaults);
    }

    myTable.prototype.set = function (o) {
        this.options = $.extend({}, o || {});
        return this;
    }

    myTable.prototype.render = function (id, url, cols, where, o) {
        var that = this;
        var d = {
            id: id,
            url: url,
            cols: cols,
            where: where
        };

        that.set(d);
        that.set(o || {});

        table.render(that.options);
    }

    myTable.prototype.reload = function (o) {
        var that = this, options = that.options;
        table.reload(options.id, o || {});
    }

    var table = new myTable();

    exports('mytable', table);
});