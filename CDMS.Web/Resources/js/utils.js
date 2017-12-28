/**
 * Name:utils.js
 * Author:Van
 * E-mail:zheng_jinfan@126.com
 * Website:http://kit.zhengjinfan.cn/
 * LICENSE:MIT
 */
layui.define(['layer'], function (exports) {
    var $ = layui.jquery, layer = top.layui.layer, _modName = 'utils';
    var version = layui.cache.version;

    var utils = {
        v: '1.0.3',

        btns: {
            btn_query: 'btn-query',//查询
            btn_add: 'btn-add',//添加
            btn_edit: 'btn-edit',//修改
            btn_delete: 'btn-delete',//删除
            btn_button: 'btn-button',//分配按钮
            btn_auth: 'btn-auth',//角色授权
            btn_authUser: 'btn-authUser',//授权用户
            btn_table: 'btn-table',//设置表名
            btn_column: 'btn-column'//设置列
        },
        /**
         * 根据一个html内容读取出body标签里的文本
         */
        getBodyContent: function (content) {
            var REG_BODY = /<body[^>]*>([\s\S]*)<\/body>/,
                result = REG_BODY.exec(content);
            if (result && result.length === 2)
                return result[1];
            return content;
        },
        /**
         * 读取html字符串
         */
        loadHtml: function (url, callback) {
            var result;
            $.ajax({
                url: url,
                async: false,
                dataType: 'html',
                beforeSend: function (request) {
                    request.setRequestHeader("IsGetHtml", 'true');
                },
                error: function (xhr, err, msg) {
                    var m = ['<div style="padding: 20px;font-size: 20px;text-align:left;color:#009688;">',
                        '<p>{{msg}}  >>> ' + url + '</p>',
                        '</div>'
                    ].join('');
                    if (xhr.status === 404) {
                        result = m.replace('{{msg}}', '<i class="layui-icon" style="font-size:70px;">&#xe61c;</i>  ' + msg);
                        return;
                    }
                    result = m.replace('{{msg}}', '<i class="layui-icon" style="font-size:70px;">&#xe69c;</i>  未知错误.');
                },
                success: function (res) {
                    result = res;
                },
                complete: function () {
                    typeof callback === 'function' && callback();
                }
            });
            return result;
        },

        post: function (url, params, callback, a) {
            return utils.ajax(url, params, callback, a);
        },
        get: function (url, params, callback, a) {
            return utils.ajax(url, params, callback, a, { method: 'get' });
        },
        /**
         * ajax post
         */
        ajax: function (url, params, callback, a, options) {
            options = options || {};
            var alertFlag = a == undefined ? true : a;
            var method = options.method || 'post';
            var dataType = options.dataType || 'json';
            var layerIndex = 0;
            $.ajax({
                url: url,
                beforeSend: function (request) {
                    request.setRequestHeader("IsAjax", 'true');
                    layerIndex = utils.load();
                },
                dataType: dataType,
                method: method,
                data: params,
                error: function (xhr, err, msg) {
                    console.log('发生错误:' + msg);
                },
                complete: function () {
                    utils.close(layerIndex);
                },
                success: function (res) {
                    if (alertFlag) {
                        var flag = res.Type <= 1;
                        var msg = res.Msg;
                        var icon = flag ? 1 : 2;
                        utils.alert(msg, icon, function (index) {
                            utils.close(index);
                            if (callback) callback(flag, msg);
                            if (flag) {
                                utils.closeFrame();
                                top.reload();
                            }
                        });
                    }
                    else callback(res);
                }
            });
        },

        keyWordHighlight: function (o, flag, rndColor, url) {
            /// <summary>
            ///     使用 javascript HTML DOM 高亮显示页面特定字词.
            ///     实例:
            ///         keyWordHighlight(document.body, '纸伞|她'); 
            ///         这里的body是指高亮body里面的内容。
            ///         keyWordHighlight(document.body, '希望|愁怨', false, '/'); 
            ///         keyWordHighlight(document.getElementById('at_main'), '独自|飘过|悠长', true, 'search.asp?keyword='); 
            ///         这里的'at_main'是指高亮id='at_main'的div里面的内容。search.asp?keyword=指给关键字加的链接地址，
            ///         我这里加了一个参数，在后面要用到。可以是任意的地址。        
            /// </summary>
            /// <param name="o" type="Object">
            ///     对象, 要进行高亮显示的对象. 
            /// </param>
            /// <param name="flag" type="String">
            ///     字符串, 要进行高亮的词或多个词, 使用 竖杠(|) 分隔多个词 . 
            /// </param>
            /// <param name="rndColor" type="Boolean">
            ///     布尔值, 是否随机显示文字背景色与文字颜色, true 表示随机显示. 
            /// </param>
            /// <param name="url" type="String">
            ///     URI, 是否对高亮的词添加链接.
            /// </param>                        
            /// <return></return>
            var bgCor = fgCor = '';
            if (rndColor) {
                bgCor = fRndCor(10, 20);
                fgCor = fRndCor(230, 255);
            } else {
                bgCor = 'transparent'; //背景色
                fgCor = 'red'; //字体颜色
            }
            var re = new RegExp(flag, 'i');
            for (var i = 0; i < o.childNodes.length; i++) {
                var o_ = o.childNodes[i];
                var o_p = o_.parentNode;
                if (o_.nodeType == 1) {
                    this.keyWordHighlight(o_, flag, rndColor, url);
                } else if (o_.nodeType == 3) {
                    if (!(o_p.nodeName == 'A')) {
                        if (o_.data.search(re) == -1) continue;
                        var temp = fEleA(o_.data, flag);
                        o_p.replaceChild(temp, o_);
                    }
                }
            }
            //------------------------------------------------ 
            function fEleA(text, flag) {
                var style = ' style="background-color:' + bgCor + ';color:' + fgCor + ';" '
                var o = document.createElement('span');
                var str = '';
                var re = new RegExp('(' + flag + ')', 'gi');
                if (url) {
                    str = text.replace(re, '<a href="' + url +
                        '$1"' + style + '>$1</a>'); //这里是给关键字加链接，红色的$1是指上面链接地址后的具体参数。
                } else {
                    str = text.replace(re, '<span ' + style + '>$1</span>'); //不加链接时显示
                }
                o.innerHTML = str;
                return o;
            }
            //------------------------------------------------ 
            function fRndCor(under, over) {
                if (arguments.length == 1) {
                    var over = under;
                    under = 0;
                } else if (arguments.length == 0) {
                    var under = 0;
                    var over = 255;
                }
                var r = fRandomBy(under, over).toString(16);
                r = padNum(r, r, 2);
                var g = fRandomBy(under, over).toString(16);
                g = padNum(g, g, 2);
                var b = fRandomBy(under, over).toString(16);
                b = padNum(b, b, 2);
                //defaultStatus=r+' '+g+' '+b 
                return '#' + r + g + b;

                function fRandomBy(under, over) {
                    switch (arguments.length) {
                        case 1:
                            return parseInt(Math.random() * under + 1);
                        case 2:
                            return parseInt(Math.random() * (over - under + 1) + under);
                        default:
                            return 0;
                    }
                }

                function padNum(str, num, len) {
                    var temp = ''
                    for (var i = 0; i < len; temp += num, i++);
                    return temp = (temp += str).substr(temp.length - len);
                }
            }
        }
        ,
        alert: function (msg, icon, yes) {
            return layer.alert(msg, { title: '提示', icon: icon || 2 }, yes);
        },
        msg: function (text) {
            return layer.msg(text, { icon: 5, shift: 6 });
        },
        load: function (type) {
            type = type || 0;
            return layer.load(type, { shade: [0.3, '#333'] });
        },
        close: function (index) {
            layer.close(index);
        },
        closeAll: function () {
            layer.closeAll();
        },
        closeFrame: function () {
            var index = layer.getFrameIndex(window.name);
            layer.close(index);
        },
        getChildFrame: function (selector, index) {
            return layer.getChildFrame(selector, index);
        },
        full: function (index) {
            layer.full(index);
        },
        open: function (options) {
            var defaults = {
                id: null,
                type: 2,
                title: '系统窗口',
                width: "100px",
                height: "100px",
                content: options.url || '',
                shade: 0.3,
                //btn: ['确认', '关闭'],
                btn: false,
                yes: function (index) {
                    if (options.callback) {
                        var frameId = 'layui-layer-iframe' + index;
                        var x = top.frames[frameId];
                        var doc = $(x.document || x);
                        options.callback(utils.getFormData(doc), index, doc);
                    }
                    else {
                        layer.close(index);
                    }
                }
            };
            var o = $.extend(defaults, options);
            return layer.open(o);
        },

        confirm: function (msg, yes, cancel, o) {
            var options = $.extend(true, { icon: 3, title: '询问' }, o);
            layer.confirm(msg, options, function (index) {
                if (yes) yes(index);
                layer.close(index);
            });
        },

        getFormData: function (doc) {//find('form')
            var array = $(doc).not('.ignore').find('input,select,textarea').serializeArray();
            var data = {};
            $.each(array, function (i, item) {
                data[item.name] = item.value;
            });
            return data;
        },
        setFormData: function (json, selector) {
            selector = selector || 'form';
            var data;
            if (typeof (json) === 'string') {
                data = JSON.parse(json);
            } else data = json;

            var form = $(selector)
            for (var key in data) {
                var input = form.find('#' + key);
                if (input.length < 1) input = form.find('[name="' + key + '"]');
                if (input.length < 1) continue;
                var v = input.attr('id') || input.attr('name');
                if (!v) continue;

                var type = input.attr('type');
                var value = $.trim(data[key]).replace(/&nbsp;/g, '');
                var tagName = input[0].tagName;
                var tagFlag = true;

                switch (tagName) {
                    case 'TEXTAREA':
                        var dataType = input.attr('data-type');
                        if (dataType == 'html')
                            value = decodeURIComponent(value);
                        input.text(value);
                        break;
                    case 'SELECT':
                        input.val(value);
                        break;
                    default:
                        tagFlag = false;
                        break;
                }
                //if (tagFlag) return;
                switch (type) {
                    case 'radio':
                        input.prop('checked', false);
                        form.find('[name="' + key + '"][value="' + value + '"]').prop("checked", true);
                        break;
                    case "checkbox":
                        if (data[key] == true || parseInt(value, 10) == 1 || value == 'Y') {
                            input.attr("checked", 'checked').val(value);
                        } else {
                            input.removeAttr("checked").val(value);
                        }
                        break;
                    case "text":
                    case "hidden":
                        var dataType = input.attr('data-type');
                        if (dataType) {
                            if (dataType == 'date') {
                                var fmt = input.attr('data-type-format') || 'yyyy-MM-dd';
                                input.val(utils.formatDate(value, fmt));
                            }
                        }
                        else input.val(value);
                        break;
                }
            }
            return data;
        },
        formatDate: function (v, format) {
            if (!v) return "";
            var d = v;
            if (typeof v === 'string') {
                if (v.indexOf("/Date(") > -1)
                    d = new Date(parseInt(v.replace("/Date(", "").replace(")/", ""), 10));
                else
                    d = new Date(Date.parse(v.replace(/-/g, "/").replace("T", " ").split(".")[0]));//.split(".")[0] 用来处理出现毫秒的情况，截取掉.xxx，否则会出错
            }
            var o = {
                "M+": d.getMonth() + 1,  //month
                "d+": d.getDate(),       //day
                "h+": d.getHours(),      //hour
                "m+": d.getMinutes(),    //minute
                "s+": d.getSeconds(),    //second
                "q+": Math.floor((d.getMonth() + 3) / 3),  //quarter
                "S": d.getMilliseconds() //millisecond
            };
            if (/(y+)/.test(format)) {
                format = format.replace(RegExp.$1, (d.getFullYear() + "").substr(4 - RegExp.$1.length));
            }
            for (var k in o) {
                if (new RegExp("(" + k + ")").test(format)) {
                    format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
                }
            }
            return format;
        },
        getQueryString: function (key, dv) {
            var result = location.search.match(new RegExp("[\?\&]" + key + "=([^\&]+)", "i"));
            if (result == null || result.length < 1) {
                return dv || '';
            }
            return result[1];
        },
        skin: {
            skinCookieName: 'CDMS_UI',
            setSkin: function (value) {
                utils.cookie(this.skinCookieName, value, { expires: 30 });
            },
            getSkin: function () {
                return utils.cookie(this.skinCookieName);
            },
            switchSkin: function (skin) {
                var current = this.getSkin();
                if (skin == current) return;

                this.setSkin(skin);

                var ui = $('#uiStyle');
                var rand = Math.random();
                var path = '/Resources/css/themes/' + skin + '.css?v=' + rand;
                ui.attr('href', path);
                if (window.frames.length > 0) {
                    var frms = window.document.getElementsByTagName("iframe");
                    $.each(frms, function (i, v) {
                        var element = v.contentWindow.document.getElementById("uiStyle")
                        $(element).attr('href', path);
                    });
                }
            }
        },

        cookie: function (name, value, options) {
            if (typeof value != 'undefined') { // name and value given, set cookie
                options = options || {};
                if (value === null) {
                    value = '';
                    options.expires = -1;
                }
                var expires = '';
                if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
                    var date;
                    if (typeof options.expires == 'number') {
                        date = new Date();
                        date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
                    } else {
                        date = options.expires;
                    }
                    expires = '; expires=' + date.toUTCString(); // use expires attribute, max-age is not supported by IE
                }
                var path = options.path ? '; path=' + options.path : '';
                var domain = options.domain ? '; domain=' + options.domain : '';
                var secure = options.secure ? '; secure' : '';
                document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
            } else { // only name given, get cookie
                var cookieValue = null;
                if (document.cookie && document.cookie != '') {
                    var cookies = document.cookie.split(';');
                    for (var i = 0; i < cookies.length; i++) {
                        var cookie = $.trim(cookies[i]);
                        // Does this cookie string begin with the name we want?
                        if (cookie.substring(0, name.length + 1) == (name + '=')) {
                            cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                            break;
                        }
                    }
                }
                return cookieValue;
            }
        }
    };


    exports('utils', utils);
});