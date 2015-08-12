window.onpopstate = function (event) { OutputState(event.state); };
//window.onkey...
var key = { ctrl: false, shift: false }
key.change = function (e, down) {
    if (e.keyCode === 17) {
        key.ctrl = down;
    } else if (e.keyCode === 16) {
        key.shift = down;
    }
}
window.onkeydown = function (e) { key.change(e, true) };
window.onkeyup = function (e) { key.change(e, false) };

var v = (function (v) {
    v.email = function (e) {
        return /[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/.test(e);
    }

    return v;
})(v ? v : {});

var util = (function (util) {
    // showing/hiding CSS
    util.hide = function (elementIDs) {
        if (Array.isArray(elementIDs)) {
            for (var i = 0, l = elementIDs.length; i < l; i++) {
                var e = util.get(elementIDs[i]);
                if (e) { e.classList.add("hide") }
            }
        } else {
            var e = util.get(elementIDs);
            if (e) { e.classList.add("hide") }
        }
    }
    util.unhide = function (elementIDs) {
        if (Array.isArray(elementIDs)) {
            for (var i = 0, l = elementIDs.length; i < l; i++) {
                var e = util.get(elementIDs[i]);
                if (e) { e.classList.remove("hide") }
            }
        } else {
            var e = util.get(elementIDs);
            if (e) { e.classList.remove("hide") }
        }
    }
    // creating DOM elements
    util.create = function (tagName, cssClass) {
        var e = document.createElement(tagName);
        if (cssClass != null) {
            e.setAttribute("class", cssClass)
        }
        return e;
    }
    // getting DOM elements
    util.get = function (elementID) {
        return document.getElementById(elementID);
    }

    // STRUCTURES //
    util.callback = function () {
        var f = [];
        var fst = null;
        this.add = function (callback) {
            f[f.length] = callback;
        };
        this.complete = function (lastCallback) {
            if (fst != null) {
                fst();
            }
            var len = f.length;
            if (len > 0) {
                for (var i = 0; i < len; i++) {
                    if (f[i] != null) {
                        f[i]();
                    }
                }
            }
            if (lastCallback && (typeof lastCallback === 'function')) {
                lastCallback();
            }
        };
        this.first = function (firstCallback) {
            fst = firstCallback;
        };
    };
    return util;
})(util ? util : {});


var f = (function () {
    this.cc = function (c, e) {
        var output = function (v) {
            v = v.split(' ').join('');
            if (v.indexOf('34') === 0 || v.indexOf('37') === 0) {
                var l = v.length > 15 ? 15 : v.length;
                // American Express
                if (l > 10) {
                    v = v.substring(0, 4) + '  ' + v.substring(4, 10) + '  ' + v.substring(10, l);
                } else if (l > 4) {
                    v = v.substring(0, 4) + '  ' + v.substring(4, l);
                }
            } else {
                var l = v.length > 16 ? 16 : v.length;
                if (l > 12) {
                    v = v.substring(0, 4) + '  ' + v.substring(4, 8) + '  ' + v.substring(8, 12) + '  ' + v.substring(12, l);
                } else if (l > 8) {
                    v = v.substring(0, 4) + '  ' + v.substring(4, 8) + '  ' + v.substring(8, l);
                } else if (v.length > 4) {
                    v = v.substring(0, 4) + '  ' + v.substring(4, l);
                }
            }
            c.value = v;
        }

        if (!key.ctrl) {
            var value = c.value;
            if (e.keyCode === 8 /* backspace */ || e.keyCode === 46 /* del */) {
                //console.log(c.selectionStart+"/"+c.selectionEnd);
                if (c.selectionEnd > 0 || e.keyCode === 46) { // nothing to delete...(Del works from first position)
                    var start = c.selectionStart;
                    var end = c.selectionEnd;
                    var length = end - start;
                    if (length > 0) {
                        value = value.substring(0, start) + value.substring(end, value.length);
                        output(value);
                        c.selectionStart = start;
                        c.selectionEnd = start;
                    } else {
                        // if basic backspace or delete, deletion moves in different direction
                        if (e.keyCode === 8) {
                            // backspace
                            value = value.substring(0, start - 1) + value.substring(end, value.length);
                            output(value);
                            c.selectionStart = start - 1;
                            c.selectionEnd = start - 1;
                        } else {
                            value = value.substring(0, start) + value.substring(end + 1, value.length);
                            output(value);
                            c.selectionStart = start;
                            c.selectionEnd = start;
                        }
                    }
                }
                return false;
            } else if ((e.keyCode >= 37 && e.keyCode <= 40) /*arrows*/) {
                return true;
            } else if (e.keyCode === 9 /*tab*/ || e.keyCode === 36 /*home*/ || e.keyCode === 35 /*end*/) {
                return true;
            } else if (e.keyCode >= 48 && e.keyCode <= 57) /* numbers */ {
                var l = c.value.length;
                if (l >= 22) {
                    var start = c.selectionStart;
                    var end = c.selectionEnd;
                    if (end > start) { // allow substitution even when at max length as long as no character increase
                        value = c.value;
                        output(value.substring(0, start) + String.fromCharCode(e.keyCode) + value.substring(end, l));
                        c.selectionStart = start + 1;
                        c.selectionEnd = c.selectionStart;
                        return false;
                    } else {
                        return false; // max length
                    }
                } else {
                    //console.log(c.selectionStart + "/b/" + l);
                    if (c.selectionStart === l) {
                        output(c.value + String.fromCharCode(e.keyCode));
                        return false;
                    } else {
                        var value = c.value;
                        var start = c.selectionStart;
                        var end = c.selectionEnd;
                        //console.log(value.substring(0, start)+"/w4t/"+value.substring(end, l));
                        value = value.substring(0, start) + String.fromCharCode(e.keyCode) + value.substring(end, l);
                        output(value);
                        start++; // add character to index to compensate for new character
                        if (c.value[start] === " ") {
                            start++;
                        }
                        // add another character if previous character was also a space
                        if (c.value[start - 1] === " ") {
                            start++;
                        }
                        // final check to make sure the current index doesn't contain a space
                        if (c.value[start] === " ") {
                            start++;
                        }
                        c.selectionStart = start;
                        c.selectionEnd = start;
                        return false;
                    }
                }
            } else {
                return false;
            }
        } else {
            // a=65,c=67,v=86,x=88
            if (e.keyCode === 88) {
                var value = c.value;
                var start = c.selectionStart;
                var end = c.selectionEnd;
                value = value.substring(0, start) + value.substring(end, value.length);
                output(value);
                c.selectionStart = start;
                c.selectionEnd = start;
                return false;
            } else if (e.keyCode === 86) {
                return true; //return false;
            } else {
                return true;
            }
        }
    }
    this.cc_paste = function (c, e) {
        var output = function (v) {
            v = v.split(' ').join('');
            if (v.indexOf('34') === 0 || v.indexOf('37') === 0) {
                var l = v.length > 15 ? 15 : v.length;
                // American Express
                if (l > 10) {
                    v = v.substring(0, 4) + '  ' + v.substring(4, 10) + '  ' + v.substring(10, l);
                } else if (l > 4) {
                    v = v.substring(0, 4) + '  ' + v.substring(4, l);
                }
            } else {
                var l = v.length > 16 ? 16 : v.length;
                if (l > 12) {
                    v = v.substring(0, 4) + '  ' + v.substring(4, 8) + '  ' + v.substring(8, 12) + '  ' + v.substring(12, l);
                } else if (l > 8) {
                    v = v.substring(0, 4) + '  ' + v.substring(4, 8) + '  ' + v.substring(8, l);
                } else if (v.length > 4) {
                    v = v.substring(0, 4) + '  ' + v.substring(4, l);
                }
                c.value = v;
            }
        };

        var value = e.clipboardData.getData("text/plain");
        var len = value.length;
        if (len > 0) {
            var numbers = "";
            for (var i = 0; i < len; i++) {
                var char = value.charAt(i);
                if ("0123456789".indexOf(char) > -1) {
                    numbers += char;
                }
            }
            //console.log(numbers);
            var va = c.value;
            var le = c.value.length;
            var start = c.selectionStart;
            var end = c.selectionEnd;
            va = va.substring(0, start) + numbers + va.substring(end, le);
            output(va);
            c.selectionStart = start;
            c.selectionEnd = c.selectionStart;
            return false;
        }
        return false;
    }
    this.mask = function (c, e) {
        //if (c.dataset.mask) {
        //    //console.log(c.value + "/" + e.keyCode);
        //} else {
        //    return true;
        //}
        return true;
    }
    this.maxlength = function (c, e) {
        if (key.ctrl || e.keyCode === 8 /* backspace */ || e.keyCode === 46 /* del */ || (e.keyCode >= 37 && e.keyCode <= 40) /*arrows*/ || e.keyCode === 9 /*tab*/ || e.keyCode === 36 /*home*/ || e.keyCode === 35 /*end*/) {
            return true;
        } else {
            var m = c.dataset.maxlength;
            var l = c.contentEditable ? c.textContent.length : c.value.length;
            if (l >= m) {
                return false;
            }
        }
    }
    this.maxlength_up = function (c, e) {
        var l = c.contentEditable ? c.textContent.length : c.value.length;
        c.dataset.displayChars = parseInt(c.dataset.maxlength) - l;
    }
    this.maxlength_paste = function (c, e) {
        var text = c.contentEditable ? c.textContent : c.value;
        var l = c.contentEditable ? c.textContent.length : text.length;
        var ml = parseInt(c.dataset.maxlength);
        if (l >= ml) {
        } else {
            var clipboard = e.clipboardData.getData("text/plain");
            if (l + clipboard.length <= ml) {
                var start = c.selectionStart;
                var end = c.selectionEnd;
                //console.log(start + "/" + end);
                text = text.substring(0, start) + clipboard + text.substring(end, l);
                c.dataset.displayChars = ml - text.length;
                if (c.contentEditable) {
                    c.innerHTML = text;
                } else {
                    c.value = text;
                }
                c.selectionStart = start;
                c.selectionEnd = c.selectionStart;

            }
        }
        return false;
    }

    return this;
})();

var lastSelected;
var navigating = false;

function link(linkDOM) {
    if (history.pushState) {
        if (lastSelected == null || linkDOM.id !== lastSelected.id) { // don't do anything if clicking same link...
            if (!navigating) {
                navigating = true;

                var r = new XMLHttpRequest();
                r.addEventListener("load", function (response) {
                    if (r.readyState == 4) {
                        var d = JSON.parse(r.responseText);
                        var state = { href: linkDOM.href, title: d.title, hrefid: linkDOM.id, content: d.content, header: d.header, key: d.key };
                        for (p in linkDOM.dataset) {
                            state[p] = linkDOM.dataset[p];
                        }
                        history.pushState(state, state.title, state.href);
                        OutputState(state, linkDOM);
                        navigating = false;
                    }
                });
                r.addEventListener("error", function (response) {
                }, false);
                r.open('GET', "/json/" + linkDOM.dataset.page);
                r.send(null);
            }
        }
        return false;
    } else {
        return true;
    }
}

var nav = document.getElementById('n');
function OutputState(state, linkDOM) {
    if (state) {
        OutputContent(state.content, state.title, state.header);
        nav.dataset['key'] = state.key;
    }
}
function OutputContent(content, title, header) {
    if (title) {
        document.title = title;
    }
    var c = document.getElementById('content');
    c.innerHTML = content;
}


function a(fid, btn) {
    btn.disabled = true;
    var btnDisabled = true;
    var f = util.get(fid); var e = f.querySelectorAll('input, select, div[contenteditable], textarea'); var ok = true; var fd = new FormData();
    for (var i = 0, l = e.length; i < l; i++) {
        var ei = e[i];
        if (ei.dataset.required && ei.value === '') {
            ok = false;
            ei.classList.add("error");
            if (ei.onkeyup == null) {
                ei.onkeyup = function (e) { if (this.value !== '') { this.classList.remove("error"); if (btnDisabled) { btn.disabled = false; btnDisabled = false } } };
            }
        } else {
            ei.classList.remove("error");
        }

        if (ei.type === 'email') {
            if (ei.value !== '' && !v.email(ei.value)) {
                ei.classList.add("error");
                ok = false;
            }
        }
        var key = ei.dataset.key;
        if (key != null) {
            if (ei.type) {
                if (ei.type === 'checkbox') {
                    fd.append(key, ei.checked);
                } else if (ei.type === 'radio') {
                    if (ei.checked) {
                        fd.append(key, ei.value);
                    }
                } else {
                    fd.append(key, ei.value);
                }
            } else if (ei.value) { // textarea
                fd.append(key, ei.value);
            } else if (ei.contentEditable) {
                fd.append(key, ei.innerHTML);
            }
        }
    }
    var e = util.get(fid + '_error');
    if (ok) {
        e.classList.add("hide");
        // submit...
        var r = new XMLHttpRequest();
        r.addEventListener("load", function (response) {
            try {
                var o = JSON.parse(this.responseText);
                if (o.ok) {
                    if (o.message) {
                        e.innerHTML = o.message;
                        e.classList.remove("hide");
                        if (o.reenable) { btn.disabled = false; }
                    } else if (o.form) {
                        var tempDiv = util.create('div');
                        tempDiv.innerHTML = o.form;
                        f.parentNode.replaceChild(tempDiv.firstChild, f);
                    } else if (o.script) {
                        f.innerHTML = "";
                        var script = document.createElement('script');
                        script.text = o.script;
                        f.appendChild(script);
                    } else if (o.substitute) {
                        var s = util.get(o.substitute.id);
                        if (s) { s.innerHTML = o.substitute.value; }
                        if (o.reenable) { btn.disabled = false; }
                    } else if (o.push) {
                        var state = o.push.state;
                        history.pushState(state, state.title, state.href);
                        OutputState(state, null);
                    } else {
                        f.innerHTML = "OK";
                    }
                } else {
                    e.innerHTML = o.message ? o.message : "Unknown Error...";
                    if (o.reenable) { btn.disabled = false; }
                    e.classList.remove("hide");
                }
            } catch (ex) {
                e.innerHTML = "Unknown Server Error...";
                e.classList.remove("hide");
                console.log(ex.message);
            }
        }, false);
        r.addEventListener("error", function (response) {
            e.innerHTML = "Error....";
            e.classList.remove("hide");
        }, false);
        r.open('POST', "/" + f.dataset.action);
        r.send(fd);
    } else {
        e.innerHTML = "Please fill required fields.";
        btn.disabled = false;
        btnDisabled = false;
        e.classList.remove("hide");
    }
};