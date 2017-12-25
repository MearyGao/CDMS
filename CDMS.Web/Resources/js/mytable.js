
layui.define(['jquery', 'table'], function (exports) {
    var $ = layui.jquery, table = layui.table;

    function myTable() {
        var tableId = 'myTable';
        var defaults = {
            id: tableId,
            url: '',
            method: 'post',
            tr_click_checkable: true,
            page: true,
            limit: 10,
            limits: [10, 20, 30]
        };
        this.options = {};
        this.set(defaults);
    }

    myTable.prototype.set = function (o) {
        this.options = $.extend(this.options, o || {});
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
        that.set({ elem: '#' + that.options.id });

        //console.log(that.options);
        table.render(that.options);
    }

    myTable.prototype.reload = function (o) {
        var that = this, options = that.options;
        table.reload(options.id, o || {});
    }

    myTable.prototype.checkStatus = function () {
        var that = this, options = that.options;
        return table.checkStatus(options.id).data;
    }

    myTable.prototype.on = function (ename, callback) {
        table.on(ename, callback);
    }

    var mytable = new myTable();

    exports('mytable', mytable);
});