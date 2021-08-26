/**
 * backbone.js + d3.js + sizzle.js
 */

 /*
                                                             ??II??=??
                                                           +=:=II+=:~=I:
                                                          ?~,:?II=:,,=+II
                                                         I?::=?II==,,==II,
                                                         I+==?III===:=?II+
                                                        =I?=??III?====?III
                                                        =III?IIII??++??III
                                                         IIIIIIIII???IIII=
                                                         ,7IIIIIIII7IIII$
                                                           ~77IIIIIIIIZ
                                                              ~?III+,
                                                         ,,,,:~=??$$$$$?+~,

   :+I=,     =I+~     ~:   +=     :~  :++,       :++~     ~:   ~      ~=        :=~         ~=:
?MMMNOON: NMMNZOMMM= +MMMMNNMMMO,,MMMMNNMMMM  7MMN77MMM+ ZMMMMMM? OMM~,,IMD  NMN, ,DMM+= 8M8 ,:MMM?=
MMM      7MM$    MMM +MMM   :MMM:,MMM    MMM?:MMD,,,,NMM ZMMM    IMM     7M? MM:   ,MM   MM,   ,MM
MM8      OMM=    MMM +MMM    MMM~,MMN    MMMZ~MMNDDDDDDD ZMMN    ZMM$ZZZZ$$? =MMZ7$MM~   ~MM$IZMM=
NMMZ   , ~MMM   ?MM8 +MMM  ,NMMN ,MMN  ,MMMM  MMMI    :  ZMMN     MM+        DM          DM
 OMMMMMMD ,MMMMMMM7  +MMMMMMMM~  ,MMMMMMMMI    DMMMMMMM+ ZMMN      DMMMMMMN  $MMMMMNDO   ZMMMMMMNZ
                      +MMM        ,MMN                                       =MD      MM:~MN      MM~
                      +MMM        ,MMN                                       ?MM?,,,$MMZ IMM=,,:?MMD
                      ~ZZ$         ZZ$                                         +DNMDI,     +8MND?,


 Copyright 2011, CopperEgg Corporation, All rights reserved.
 http://copperegg.com

 Includes jQuery JavaScript Library
 Copyright 2011, John Resig
 Dual licensed under the MIT or GPL Version 2 licenses.
 http://jquery.org/license

 Includes Sizzle.js
 http://sizzlejs.com/
 Copyright 2011, The Dojo Foundation
 Released under the MIT, BSD, and GPL Licenses.

 Includes Backbone.js
 (c) 2010 Jeremy Ashkenas, DocumentCloud Inc.
 Backbone may be freely distributed under the MIT license.
 For all details and documentation:
 http://documentcloud.github.com/backbone

 Includes Underscore.js
 (c) 2011 Jeremy Ashkenas, DocumentCloud Inc.
 Underscore is freely distributable under the MIT license.
 Portions of Underscore are inspired or borrowed from Prototype,
 Oliver Steele's Functional, and John Resig's Micro-Templating.
 For all details and documentation:
 http://documentcloud.github.com/underscore

 CopperEgg ASCII art logo generated from http://www.glassgiant.com/ascii/

*/
//backbone starts
(function() {
    var C = this;
    var z = C._;
    var c = {};
    var n = Array.prototype, I = Object.prototype, L = Function.prototype;
    var A = n.slice, E = n.unshift, D = I.toString, u = I.hasOwnProperty;
    var s = n.forEach, l = n.map, G = n.reduce, g = n.reduceRight, q = n.filter, a = n.every, F = n.some, B = n.indexOf, h = n.lastIndexOf, d = Array.isArray, H = Object.keys, o = L.bind;
    var K = function(N) {
        return new j(N)
    };
    if (typeof exports !== "undefined") {
        if (typeof module !== "undefined" && module.exports) {
            exports = module.exports = K
        }
        exports._ = K
    } else {
        if (typeof define === "function" && define.amd) {
            define("underscore", function() {
                return K
            })
        } else {
            C._ = K
        }
    }
    K.VERSION = "1.2.2";
    var f = K.each = K.forEach = function(S, R, Q) {
        if (S == null) {
            return
        }
        if (s && S.forEach === s) {
            S.forEach(R, Q)
        } else {
            if (S.length === +S.length) {
                for (var P = 0, N = S.length; P < N; P++) {
                    if (P in S && R.call(Q, S[P], P, S) === c) {
                        return
                    }
                }
            } else {
                for (var O in S) {
                    if (u.call(S, O)) {
                        if (R.call(Q, S[O], O, S) === c) {
                            return
                        }
                    }
                }
            }
        }
    };
    K.map = function(Q, P, O) {
        var N = [];
        if (Q == null) {
            return N
        }
        if (l && Q.map === l) {
            return Q.map(P, O)
        }
        f(Q, function(T, R, S) {
            N[N.length] = P.call(O, T, R, S)
        });
        return N
    };
    K.reduce = K.foldl = K.inject = function(R, Q, N, P) {
        var O = N !== void 0;
        if (R == null) {
            R = []
        }
        if (G && R.reduce === G) {
            if (P) {
                Q = K.bind(Q, P)
            }
            return O ? R.reduce(Q, N) : R.reduce(Q)
        }
        f(R, function(U, S, T) {
            if (!O) {
                N = U;
                O = true
            } else {
                N = Q.call(P, N, U, S, T)
            }
        });
        if (!O) {
            throw new TypeError("Reduce of empty array with no initial value")
        }
        return N
    };
    K.reduceRight = K.foldr = function(Q, P, N, O) {
        if (Q == null) {
            Q = []
        }
        if (g && Q.reduceRight === g) {
            if (O) {
                P = K.bind(P, O)
            }
            return N !== void 0 ? Q.reduceRight(P, N) : Q.reduceRight(P)
        }
        var R = (K.isArray(Q) ? Q.slice() : K.toArray(Q)).reverse();
        return K.reduce(R, P, N, O)
    };
    K.find = K.detect = function(Q, P, O) {
        var N;
        v(Q, function(T, R, S) {
            if (P.call(O, T, R, S)) {
                N = T;
                return true
            }
        });
        return N
    };
    K.filter = K.select = function(Q, P, O) {
        var N = [];
        if (Q == null) {
            return N
        }
        if (q && Q.filter === q) {
            return Q.filter(P, O)
        }
        f(Q, function(T, R, S) {
            if (P.call(O, T, R, S)) {
                N[N.length] = T
            }
        });
        return N
    };
    K.reject = function(Q, P, O) {
        var N = [];
        if (Q == null) {
            return N
        }
        f(Q, function(T, R, S) {
            if (!P.call(O, T, R, S)) {
                N[N.length] = T
            }
        });
        return N
    };
    K.every = K.all = function(Q, P, O) {
        var N = true;
        if (Q == null) {
            return N
        }
        if (a && Q.every === a) {
            return Q.every(P, O)
        }
        f(Q, function(T, R, S) {
            if (!(N = N && P.call(O, T, R, S))) {
                return c
            }
        });
        return N
    };
    var v = K.some = K.any = function(Q, P, O) {
        P = P || K.identity;
        var N = false;
        if (Q == null) {
            return N
        }
        if (F && Q.some === F) {
            return Q.some(P, O)
        }
        f(Q, function(T, R, S) {
            if (N || (N = P.call(O, T, R, S))) {
                return c
            }
        });
        return !!N
    };
    K.include = K.contains = function(P, O) {
        var N = false;
        if (P == null) {
            return N
        }
        if (B && P.indexOf === B) {
            return P.indexOf(O) != -1
        }
        N = v(P, function(Q) {
            return Q === O
        });
        return N
    };
    K.invoke = function(O, P) {
        var N = A.call(arguments, 2);
        return K.map(O, function(Q) {
            return (P.call ? P || Q : Q[P]).apply(Q, N)
        })
    };
    K.pluck = function(O, N) {
        return K.map(O, function(P) {
            return P[N]
        })
    };
    K.max = function(Q, P, O) {
        if (!P && K.isArray(Q)) {
            return Math.max.apply(Math, Q)
        }
        if (!P && K.isEmpty(Q)) {
            return -Infinity
        }
        var N = {computed: -Infinity};
        f(Q, function(U, R, T) {
            var S = P ? P.call(O, U, R, T) : U;
            S >= N.computed && (N = {value: U,computed: S})
        });
        return N.value
    };
    K.min = function(Q, P, O) {
        if (!P && K.isArray(Q)) {
            return Math.min.apply(Math, Q)
        }
        if (!P && K.isEmpty(Q)) {
            return Infinity
        }
        var N = {computed: Infinity};
        f(Q, function(U, R, T) {
            var S = P ? P.call(O, U, R, T) : U;
            S < N.computed && (N = {value: U,computed: S})
        });
        return N.value
    };
    K.shuffle = function(P) {
        var N = [], O;
        f(P, function(S, Q, R) {
            if (Q == 0) {
                N[0] = S
            } else {
                O = Math.floor(Math.random() * (Q + 1));
                N[Q] = N[O];
                N[O] = S
            }
        });
        return N
    };
    K.sortBy = function(P, O, N) {
        return K.pluck(K.map(P, function(S, Q, R) {
            return {value: S,criteria: O.call(N, S, Q, R)}
        }).sort(function(T, S) {
            var R = T.criteria, Q = S.criteria;
            return R < Q ? -1 : R > Q ? 1 : 0
        }), "value")
    };
    K.groupBy = function(P, Q) {
        var N = {};
        var O = K.isFunction(Q) ? Q : function(R) {
            return R[Q]
        };
        f(P, function(T, R) {
            var S = O(T, R);
            (N[S] || (N[S] = [])).push(T)
        });
        return N
    };
    K.sortedIndex = function(S, R, P) {
        P || (P = K.identity);
        var N = 0, Q = S.length;
        while (N < Q) {
            var O = (N + Q) >> 1;
            P(S[O]) < P(R) ? N = O + 1 : Q = O
        }
        return N
    };
    K.toArray = function(N) {
        if (!N) {
            return []
        }
        if (N.toArray) {
            return N.toArray()
        }
        if (K.isArray(N)) {
            return A.call(N)
        }
        if (K.isArguments(N)) {
            return A.call(N)
        }
        return K.values(N)
    };
    K.size = function(N) {
        return K.toArray(N).length
    };
    K.first = K.head = function(P, O, N) {
        return (O != null) && !N ? A.call(P, 0, O) : P[0]
    };
    K.initial = function(P, O, N) {
        return A.call(P, 0, P.length - ((O == null) || N ? 1 : O))
    };
    K.last = function(P, O, N) {
        if ((O != null) && !N) {
            return A.call(P, Math.max(P.length - O, 0))
        } else {
            return P[P.length - 1]
        }
    };
    K.rest = K.tail = function(P, N, O) {
        return A.call(P, (N == null) || O ? 1 : N)
    };
    K.compact = function(N) {
        return K.filter(N, function(O) {
            return !!O
        })
    };
    K.flatten = function(O, N) {
        return K.reduce(O, function(P, Q) {
            if (K.isArray(Q)) {
                return P.concat(N ? Q : K.flatten(Q))
            }
            P[P.length] = Q;
            return P
        }, [])
    };
    K.without = function(N) {
        return K.difference(N, A.call(arguments, 1))
    };
    K.uniq = K.unique = function(R, Q, P) {
        var O = P ? K.map(R, P) : R;
        var N = [];
        K.reduce(O, function(S, U, T) {
            if (0 == T || (Q === true ? K.last(S) != U : !K.include(S, U))) {
                S[S.length] = U;
                N[N.length] = R[T]
            }
            return S
        }, []);
        return N
    };
    K.union = function() {
        return K.uniq(K.flatten(arguments, true))
    };
    K.intersection = K.intersect = function(O) {
        var N = A.call(arguments, 1);
        return K.filter(K.uniq(O), function(P) {
            return K.every(N, function(Q) {
                return K.indexOf(Q, P) >= 0
            })
        })
    };
    K.difference = function(O, N) {
        return K.filter(O, function(P) {
            return !K.include(N, P)
        })
    };
    K.zip = function() {
        var N = A.call(arguments);
        var Q = K.max(K.pluck(N, "length"));
        var P = new Array(Q);
        for (var O = 0; O < Q; O++) {
            P[O] = K.pluck(N, "" + O)
        }
        return P
    };
    K.indexOf = function(R, P, Q) {
        if (R == null) {
            return -1
        }
        var O, N;
        if (Q) {
            O = K.sortedIndex(R, P);
            return R[O] === P ? O : -1
        }
        if (B && R.indexOf === B) {
            return R.indexOf(P)
        }
        for (O = 0, N = R.length; O < N; O++) {
            if (R[O] === P) {
                return O
            }
        }
        return -1
    };
    K.lastIndexOf = function(P, O) {
        if (P == null) {
            return -1
        }
        if (h && P.lastIndexOf === h) {
            return P.lastIndexOf(O)
        }
        var N = P.length;
        while (N--) {
            if (P[N] === O) {
                return N
            }
        }
        return -1
    };
    K.range = function(S, Q, R) {
        if (arguments.length <= 1) {
            Q = S || 0;
            S = 0
        }
        R = arguments[2] || 1;
        var O = Math.max(Math.ceil((Q - S) / R), 0);
        var N = 0;
        var P = new Array(O);
        while (N < O) {
            P[N++] = S;
            S += R
        }
        return P
    };
    var k = function() {
    };
    K.bind = function M(Q, O) {
        var P, N;
        if (Q.bind === o && o) {
            return o.apply(Q, A.call(arguments, 1))
        }
        if (!K.isFunction(Q)) {
            throw new TypeError
        }
        N = A.call(arguments, 2);
        return P = function() {
            if (!(this instanceof P)) {
                return Q.apply(O, N.concat(A.call(arguments)))
            }
            k.prototype = Q.prototype;
            var S = new k;
            var R = Q.apply(S, N.concat(A.call(arguments)));
            if (Object(R) === R) {
                return R
            }
            return S
        }
    };
    K.bindAll = function(O) {
        var N = A.call(arguments, 1);
        if (N.length == 0) {
            N = K.functions(O)
        }
        f(N, function(P) {
            O[P] = K.bind(O[P], O)
        });
        return O
    };
    K.memoize = function(P, O) {
        var N = {};
        O || (O = K.identity);
        return function() {
            var Q = O.apply(this, arguments);
            return u.call(N, Q) ? N[Q] : (N[Q] = P.apply(this, arguments))
        }
    };
    K.delay = function(O, P) {
        var N = A.call(arguments, 2);
        return setTimeout(function() {
            return O.apply(O, N)
        }, P)
    };
    K.defer = function(N) {
        return K.delay.apply(K, [N, 1].concat(A.call(arguments, 1)))
    };
    K.throttle = function(S, U) {
        var Q, N, T, R, P;
        var O = K.debounce(function() {
            P = R = false
        }, U);
        return function() {
            Q = this;
            N = arguments;
            var V = function() {
                T = null;
                if (P) {
                    S.apply(Q, N)
                }
                O()
            };
            if (!T) {
                T = setTimeout(V, U)
            }
            if (R) {
                P = true
            } else {
                S.apply(Q, N)
            }
            O();
            R = true
        }
    };
    K.debounce = function(N, P) {
        var O;
        return function() {
            var S = this, R = arguments;
            var Q = function() {
                O = null;
                N.apply(S, R)
            };
            clearTimeout(O);
            O = setTimeout(Q, P)
        }
    };
    K.once = function(P) {
        var N = false, O;
        return function() {
            if (N) {
                return O
            }
            N = true;
            return O = P.apply(this, arguments)
        }
    };
    K.wrap = function(N, O) {
        return function() {
            var P = [N].concat(A.call(arguments));
            return O.apply(this, P)
        }
    };
    K.compose = function() {
        var N = A.call(arguments);
        return function() {
            var O = A.call(arguments);
            for (var P = N.length - 1; P >= 0; P--) {
                O = [N[P].apply(this, O)]
            }
            return O[0]
        }
    };
    K.after = function(O, N) {
        if (O <= 0) {
            return N()
        }
        return function() {
            if (--O < 1) {
                return N.apply(this, arguments)
            }
        }
    };
    K.keys = H || function(P) {
        if (P !== Object(P)) {
            throw new TypeError("Invalid object")
        }
        var O = [];
        for (var N in P) {
            if (u.call(P, N)) {
                O[O.length] = N
            }
        }
        return O
    };
    K.values = function(N) {
        return K.map(N, K.identity)
    };
    K.functions = K.methods = function(P) {
        var O = [];
        for (var N in P) {
            if (K.isFunction(P[N])) {
                O.push(N)
            }
        }
        return O.sort()
    };
    K.extend = function(N) {
        f(A.call(arguments, 1), function(O) {
            for (var P in O) {
                if (O[P] !== void 0) {
                    N[P] = O[P]
                }
            }
        });
        return N
    };
    K.defaults = function(N) {
        f(A.call(arguments, 1), function(O) {
            for (var P in O) {
                if (N[P] == null) {
                    N[P] = O[P]
                }
            }
        });
        return N
    };
    K.clone = function(N) {
        if (!K.isObject(N)) {
            return N
        }
        return K.isArray(N) ? N.slice() : K.extend({}, N)
    };
    K.tap = function(O, N) {
        N(O);
        return O
    };
    function J(Q, P, O) {
        if (Q === P) {
            return Q !== 0 || 1 / Q == 1 / P
        }
        if (Q == null || P == null) {
            return Q === P
        }
        if (Q._chain) {
            Q = Q._wrapped
        }
        if (P._chain) {
            P = P._wrapped
        }
        if (K.isFunction(Q.isEqual)) {
            return Q.isEqual(P)
        }
        if (K.isFunction(P.isEqual)) {
            return P.isEqual(Q)
        }
        var T = D.call(Q);
        if (T != D.call(P)) {
            return false
        }
        switch (T) {
            case "[object String]":
                return String(Q) == String(P);
            case "[object Number]":
                Q = +Q;
                P = +P;
                return Q != Q ? P != P : (Q == 0 ? 1 / Q == 1 / P : Q == P);
            case "[object Date]":
            case "[object Boolean]":
                return +Q == +P;
            case "[object RegExp]":
                return Q.source == P.source && Q.global == P.global && Q.multiline == P.multiline && Q.ignoreCase == P.ignoreCase
        }
        if (typeof Q != "object" || typeof P != "object") {
            return false
        }
        var U = O.length;
        while (U--) {
            if (O[U] == Q) {
                return true
            }
        }
        O.push(Q);
        var S = 0, N = true;
        if (T == "[object Array]") {
            S = Q.length;
            N = S == P.length;
            if (N) {
                while (S--) {
                    if (!(N = S in Q == S in P && J(Q[S], P[S], O))) {
                        break
                    }
                }
            }
        } else {
            if ("constructor" in Q != "constructor" in P || Q.constructor != P.constructor) {
                return false
            }
            for (var R in Q) {
                if (u.call(Q, R)) {
                    S++;
                    if (!(N = u.call(P, R) && J(Q[R], P[R], O))) {
                        break
                    }
                }
            }
            if (N) {
                for (R in P) {
                    if (u.call(P, R) && !(S--)) {
                        break
                    }
                }
                N = !S
            }
        }
        O.pop();
        return N
    }
    K.isEqual = function(O, N) {
        return J(O, N, [])
    };
    K.isEmpty = function(O) {
        if (K.isArray(O) || K.isString(O)) {
            return O.length === 0
        }
        for (var N in O) {
            if (u.call(O, N)) {
                return false
            }
        }
        return true
    };
    K.isElement = function(N) {
        return !!(N && N.nodeType == 1)
    };
    K.isArray = d || function(N) {
        return D.call(N) == "[object Array]"
    };
    K.isObject = function(N) {
        return N === Object(N)
    };
    if (D.call(arguments) == "[object Arguments]") {
        K.isArguments = function(N) {
            return D.call(N) == "[object Arguments]"
        }
    } else {
        K.isArguments = function(N) {
            return !!(N && u.call(N, "callee"))
        }
    }
    K.isFunction = function(N) {
        return D.call(N) == "[object Function]"
    };
    K.isString = function(N) {
        return D.call(N) == "[object String]"
    };
    K.isNumber = function(N) {
        return D.call(N) == "[object Number]"
    };
    K.isNaN = function(N) {
        return N !== N
    };
    K.isBoolean = function(N) {
        return N === true || N === false || D.call(N) == "[object Boolean]"
    };
    K.isDate = function(N) {
        return D.call(N) == "[object Date]"
    };
    K.isRegExp = function(N) {
        return D.call(N) == "[object RegExp]"
    };
    K.isNull = function(N) {
        return N === null
    };
    K.isUndefined = function(N) {
        return N === void 0
    };
    K.noConflict = function() {
        C._ = z;
        return this
    };
    K.identity = function(N) {
        return N
    };
    K.times = function(Q, P, O) {
        for (var N = 0; N < Q; N++) {
            P.call(O, N)
        }
    };
    K.escape = function(N) {
        return ("" + N).replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#x27;").replace(/\//g, "&#x2F;")
    };
    K.mixin = function(N) {
        f(K.functions(N), function(O) {
            w(O, K[O] = N[O])
        })
    };
    var p = 0;
    K.uniqueId = function(N) {
        var O = p++;
        return N ? N + O : O
    };
    K.templateSettings = {evaluate: /<%([\s\S]+?)%>/g,interpolate: /<%=([\s\S]+?)%>/g,escape: /<%-([\s\S]+?)%>/g};
    K.template = function(Q, P) {
        var R = K.templateSettings;
        var N = "var __p=[],print=function(){__p.push.apply(__p,arguments);};with(obj||{}){__p.push('" + Q.replace(/\\/g, "\\\\").replace(/'/g, "\\'").replace(R.escape, function(S, T) {
            return "',_.escape(" + T.replace(/\\'/g, "'") + "),'"
        }).replace(R.interpolate, function(S, T) {
            return "'," + T.replace(/\\'/g, "'") + ",'"
        }).replace(R.evaluate || null, function(S, T) {
            return "');" + T.replace(/\\'/g, "'").replace(/[\r\n\t]/g, " ") + ";__p.push('"
        }).replace(/\r/g, "\\r").replace(/\n/g, "\\n").replace(/\t/g, "\\t") + "');}return __p.join('');";
        var O = new Function("obj", "_", N);
        return P ? O(P, K) : function(S) {
            return O(S, K)
        }
    };
    var j = function(N) {
        this._wrapped = N
    };
    K.prototype = j.prototype;
    var t = function(O, N) {
        return N ? K(O).chain() : O
    };
    var w = function(N, O) {
        j.prototype[N] = function() {
            var P = A.call(arguments);
            E.call(P, this._wrapped);
            return t(O.apply(K, P), this._chain)
        }
    };
    K.mixin(K);
    f(["pop", "push", "reverse", "shift", "sort", "splice", "unshift"], function(N) {
        var O = n[N];
        j.prototype[N] = function() {
            O.apply(this._wrapped, arguments);
            return t(this._wrapped, this._chain)
        }
    });
    f(["concat", "join", "slice"], function(N) {
        var O = n[N];
        j.prototype[N] = function() {
            return t(O.apply(this._wrapped, arguments), this._chain)
        }
    });
    j.prototype.chain = function() {
        this._chain = true;
        return this
    };
    j.prototype.value = function() {
        return this._wrapped
    }
}).call(this);
(function() {
    var t = this;
    var s = t.Backbone;
    var c;
    if (typeof exports !== "undefined") {
        c = exports
    } else {
        c = t.Backbone = {}
    }
    c.VERSION = "0.5.1";
    var D = t._;
    if (!D && (typeof require !== "undefined")) {
        D = require("underscore")._
    }
    var g = t.jQuery || t.Zepto;
    c.noConflict = function() {
        t.Backbone = s;
        return this
    };
    c.emulateHTTP = false;
    c.emulateJSON = false;
    c.Events = {bind: function(F, H) {
            var E = this._callbacks || (this._callbacks = {});
            var G = E[F] || (E[F] = []);
            G.push(H);
            return this
        },unbind: function(H, J) {
            var G;
            if (!H) {
                this._callbacks = {}
            } else {
                if (G = this._callbacks) {
                    if (!J) {
                        G[H] = []
                    } else {
                        var I = G[H];
                        if (!I) {
                            return this
                        }
                        for (var F = 0, E = I.length; F < E; F++) {
                            if (J === I[F]) {
                                I[F] = null;
                                break
                            }
                        }
                    }
                }
            }
            return this
        },trigger: function(G) {
            var H, M, K, L, I;
            var J = 2;
            if (!(M = this._callbacks)) {
                return this
            }
            while (J--) {
                K = J ? G : "all";
                if (H = M[K]) {
                    for (var F = 0, E = H.length; F < E; F++) {
                        if (!(L = H[F])) {
                            H.splice(F, 1);
                            F--;
                            E--
                        } else {
                            I = J ? Array.prototype.slice.call(arguments, 1) : arguments;
                            L.apply(this, I)
                        }
                    }
                }
            }
            return this
        }};
    c.Model = function(E, F) {
        var G;
        E || (E = {});
        if (G = this.defaults) {
            if (D.isFunction(G)) {
                G = G()
            }
            E = D.extend({}, G, E)
        }
        this.attributes = {};
        this._escapedAttributes = {};
        this.cid = D.uniqueId("c");
        this.set(E, {silent: true});
        this._changed = false;
        this._previousAttributes = D.clone(this.attributes);
        if (F && F.collection) {
            this.collection = F.collection
        }
        this.initialize(E, F)
    };
    D.extend(c.Model.prototype, c.Events, {_previousAttributes: null,_changed: false,idAttribute: "id",initialize: function() {
        },toJSON: function() {
            return D.clone(this.attributes)
        },get: function(E) {
            return this.attributes[E]
        },escape: function(E) {
            var F;
            if (F = this._escapedAttributes[E]) {
                return F
            }
            var G = this.attributes[E];
            return this._escapedAttributes[E] = q(G == null ? "" : "" + G)
        },has: function(E) {
            return this.attributes[E] != null
        },set: function(H, G) {
            G || (G = {});
            if (!H) {
                return this
            }
            var F = this.attributes, J = this._escapedAttributes;
            if (!G.silent && this.validate && !this._performValidation(H, G)) {
                return false
            }
            if (this.idAttribute in H) {
                this.id = H[this.idAttribute]
            }
            var I = this._changing;
            this._changing = true;
            for (var E in H) {
                var K = H[E];
                if (!D.isEqual(F[E], K)) {
                    F[E] = K;
                    delete J[E];
                    this._changed = true;
                    if (!G.silent) {
                        this.trigger("change:" + E, this, K, G)
                    }
                }
            }
            if (!I && !G.silent && this._changed) {
                this.change(G)
            }
            this._changing = false;
            return this
        },unset: function(E, F) {
            if (!(E in this.attributes)) {
                return this
            }
            F || (F = {});
            var H = this.attributes[E];
            var G = {};
            G[E] = void 0;
            if (!F.silent && this.validate && !this._performValidation(G, F)) {
                return false
            }
            delete this.attributes[E];
            delete this._escapedAttributes[E];
            if (E == this.idAttribute) {
                delete this.id
            }
            this._changed = true;
            if (!F.silent) {
                this.trigger("change:" + E, this, void 0, F);
                this.change(F)
            }
            return this
        },clear: function(G) {
            G || (G = {});
            var E;
            var F = this.attributes;
            var H = {};
            for (E in F) {
                H[E] = void 0
            }
            if (!G.silent && this.validate && !this._performValidation(H, G)) {
                return false
            }
            this.attributes = {};
            this._escapedAttributes = {};
            this._changed = true;
            if (!G.silent) {
                for (E in F) {
                    this.trigger("change:" + E, this, void 0, G)
                }
                this.change(G)
            }
            return this
        },fetch: function(F) {
            F || (F = {});
            var E = this;
            var G = F.success;
            F.success = function(J, H, I) {
                if (!E.set(E.parse(J, I), F)) {
                    return false
                }
                if (G) {
                    G(E, J)
                }
            };
            F.error = d(F.error, E, F);
            return (this.sync || c.sync).call(this, "read", this, F)
        },save: function(G, F) {
            F || (F = {});
            if (G && !this.set(G, F)) {
                return false
            }
            var E = this;
            var H = F.success;
            F.success = function(L, J, K) {
                if (!E.set(E.parse(L, K), F)) {
                    return false
                }
                if (H) {
                    H(E, L, K)
                }
            };
            F.error = d(F.error, E, F);
            var I = this.isNew() ? "create" : "update";
            return (this.sync || c.sync).call(this, I, this, F)
        },destroy: function(F) {
            F || (F = {});
            if (this.isNew()) {
                return this.trigger("destroy", this, this.collection, F)
            }
            var E = this;
            var G = F.success;
            F.success = function(H) {
                E.trigger("destroy", E, E.collection, F);
                if (G) {
                    G(E, H)
                }
            };
            F.error = d(F.error, E, F);
            return (this.sync || c.sync).call(this, "delete", this, F)
        },url: function() {
            var E = u(this.collection) || this.urlRoot || w();
            if (this.isNew()) {
                return E
            }
            return E + (E.charAt(E.length - 1) == "/" ? "" : "/") + encodeURIComponent(this.id)
        },parse: function(F, E) {
            return F
        },clone: function() {
            return new this.constructor(this)
        },isNew: function() {
            return this.id == null
        },change: function(E) {
            this.trigger("change", this, E);
            this._previousAttributes = D.clone(this.attributes);
            this._changed = false
        },hasChanged: function(E) {
            if (E) {
                return this._previousAttributes[E] != this.attributes[E]
            }
            return this._changed
        },changedAttributes: function(G) {
            G || (G = this.attributes);
            var F = this._previousAttributes;
            var H = false;
            for (var E in G) {
                if (!D.isEqual(F[E], G[E])) {
                    H = H || {};
                    H[E] = G[E]
                }
            }
            return H
        },previous: function(E) {
            if (!E || !this._previousAttributes) {
                return null
            }
            return this._previousAttributes[E]
        },previousAttributes: function() {
            return D.clone(this._previousAttributes)
        },_performValidation: function(G, F) {
            var E = this.validate(G);
            if (E) {
                if (F.error) {
                    F.error(this, E, F)
                } else {
                    this.trigger("error", this, E, F)
                }
                return false
            }
            return true
        }});
    c.Collection = function(F, E) {
        E || (E = {});
        if (E.comparator) {
            this.comparator = E.comparator
        }
        D.bindAll(this, "_onModelEvent", "_removeReference");
        this._reset();
        if (F) {
            this.reset(F, {silent: true})
        }
        this.initialize.apply(this, arguments)
    };
    D.extend(c.Collection.prototype, c.Events, {model: c.Model,initialize: function() {
        },toJSON: function() {
            return this.map(function(E) {
                return E.toJSON()
            })
        },add: function(H, F) {
            if (D.isArray(H)) {
                for (var G = 0, E = H.length; G < E; G++) {
                    this._add(H[G], F)
                }
            } else {
                this._add(H, F)
            }
            return this
        },remove: function(H, F) {
            if (D.isArray(H)) {
                for (var G = 0, E = H.length; G < E; G++) {
                    this._remove(H[G], F)
                }
            } else {
                this._remove(H, F)
            }
            return this
        },get: function(E) {
            if (E == null) {
                return null
            }
            return this._byId[E.id != null ? E.id : E]
        },getByCid: function(E) {
            return E && this._byCid[E.cid || E]
        },at: function(E) {
            return this.models[E]
        },sort: function(E) {
            E || (E = {});
            if (!this.comparator) {
                throw new Error("Cannot sort a set without a comparator")
            }
            this.models = this.sortBy(this.comparator);
            if (!E.silent) {
                this.trigger("reset", this, E)
            }
            return this
        },pluck: function(E) {
            return D.map(this.models, function(F) {
                return F.get(E)
            })
        },reset: function(F, E) {
            F || (F = []);
            E || (E = {});
            this.each(this._removeReference);
            this._reset();
            this.add(F, {silent: true});
            if (!E.silent) {
                this.trigger("reset", this, E)
            }
            return this
        },fetch: function(E) {
            E || (E = {});
            var G = this;
            var F = E.success;
            E.success = function(J, H, I) {
                G[E.add ? "add" : "reset"](G.parse(J, I), E);
                if (F) {
                    F(G, J)
                }
            };
            E.error = d(E.error, G, E);
            return (this.sync || c.sync).call(this, "read", this, E)
        },create: function(F, E) {
            var G = this;
            E || (E = {});
            F = this._prepareModel(F, E);
            if (!F) {
                return false
            }
            var H = E.success;
            E.success = function(I, K, J) {
                G.add(I, E);
                if (H) {
                    H(I, K, J)
                }
            };
            F.save(null, E);
            return F
        },parse: function(F, E) {
            return F
        },chain: function() {
            return D(this.models).chain()
        },where: function(E) {
            if (D.isEmpty(E)) {
                return []
            }
            return this.filter(function(F) {
                for (var G in E) {
                    if (E[G] !== F.get(G)) {
                        return false
                    }
                }
                return true
            })
        },_reset: function(E) {
            this.length = 0;
            this.models = [];
            this._byId = {};
            this._byCid = {}
        },_prepareModel: function(G, F) {
            if (!(G instanceof c.Model)) {
                var E = G;
                G = new this.model(E, {collection: this});
                if (G.validate && !G._performValidation(E, F)) {
                    G = false
                }
            } else {
                if (!G.collection) {
                    G.collection = this
                }
            }
            return G
        },_add: function(G, F) {
            F || (F = {});
            G = this._prepareModel(G, F);
            if (!G) {
                return false
            }
            var H = this.getByCid(G) || this.get(G);
            if (H) {
                throw new Error(["Can't add the same model to a set twice", H.id])
            }
            this._byId[G.id] = G;
            this._byCid[G.cid] = G;
            var E = F.at != null ? F.at : this.comparator ? this.sortedIndex(G, this.comparator) : this.length;
            this.models.splice(E, 0, G);
            G.bind("all", this._onModelEvent);
            this.length++;
            if (!F.silent) {
                G.trigger("add", G, this, F)
            }
            return G
        },_remove: function(F, E) {
            E || (E = {});
            F = this.getByCid(F) || this.get(F);
            if (!F) {
                return null
            }
            delete this._byId[F.id];
            delete this._byCid[F.cid];
            this.models.splice(this.indexOf(F), 1);
            this.length--;
            if (!E.silent) {
                F.trigger("remove", F, this, E)
            }
            this._removeReference(F);
            return F
        },_removeReference: function(E) {
            if (this == E.collection) {
                delete E.collection
            }
            E.unbind("all", this._onModelEvent)
        },_onModelEvent: function(G, F, H, E) {
            if ((G == "add" || G == "remove") && H != this) {
                return
            }
            if (G == "destroy") {
                this._remove(F, E)
            }
            if (F && G === "change:" + F.idAttribute) {
                delete this._byId[F.previous(F.idAttribute)];
                this._byId[F.id] = F
            }
            this.trigger.apply(this, arguments)
        }});
    var B = ["forEach", "each", "map", "reduce", "reduceRight", "find", "detect", "filter", "select", "reject", "every", "all", "some", "any", "include", "contains", "invoke", "max", "min", "sortBy", "sortedIndex", "toArray", "size", "first", "rest", "last", "without", "indexOf", "lastIndexOf", "isEmpty"];
    D.each(B, function(E) {
        c.Collection.prototype[E] = function() {
            return D[E].apply(D, [this.models].concat(D.toArray(arguments)))
        }
    });
    c.Router = function(E) {
        E || (E = {});
        if (E.routes) {
            this.routes = E.routes
        }
        this._bindRoutes();
        this.initialize.apply(this, arguments)
    };
    var j = /:([\w\d]+)/g;
    var C = /\*([\w\d]+)/g;
    var f = /[-[\]{}()+?.,\\^$|#\s]/g;
    D.extend(c.Router.prototype, c.Events, {initialize: function() {
        },route: function(E, F, G) {
            c.history || (c.history = new c.History);
            if (!D.isRegExp(E)) {
                E = this._routeToRegExp(E)
            }
            c.history.route(E, D.bind(function(I) {
                var H = this._extractParameters(E, I);
                G.apply(this, H);
                this.trigger.apply(this, ["route:" + F].concat(H))
            }, this))
        },navigate: function(E, F) {
            c.history.navigate(E, F)
        },_bindRoutes: function() {
            if (!this.routes) {
                return
            }
            var F = [];
            for (var G in this.routes) {
                F.unshift([G, this.routes[G]])
            }
            for (var H = 0, E = F.length; H < E; H++) {
                this.route(F[H][0], F[H][1], this[F[H][1]])
            }
        },_routeToRegExp: function(E) {
            E = E.replace(f, "\\$&").replace(j, "([^/]*)").replace(C, "(.*?)");
            return new RegExp("^" + E + "$")
        },_extractParameters: function(E, F) {
            return E.exec(F).slice(1)
        }});
    c.History = function() {
        this.handlers = [];
        D.bindAll(this, "checkUrl")
    };
    var p = /^#*/;
    var k = /msie [\w.]+/;
    var n = false;
    D.extend(c.History.prototype, {interval: 50,getFragment: function(F, E) {
            if (F == null) {
                if (this._hasPushState || E) {
                    F = window.location.pathname;
                    var G = window.location.search;
                    if (G) {
                        F += G
                    }
                    if (F.indexOf(this.options.root) == 0) {
                        F = F.substr(this.options.root.length)
                    }
                } else {
                    F = window.location.hash
                }
            }
            return F.replace(p, "")
        },start: function(G) {
            if (n) {
                throw new Error("Backbone.history has already been started")
            }
            this.options = D.extend({}, {root: "/"}, this.options, G);
            this._wantsPushState = !!this.options.pushState;
            this._hasPushState = !!(this.options.pushState && window.history && window.history.pushState);
            var F = this.getFragment();
            var E = document.documentMode;
            var I = (k.exec(navigator.userAgent.toLowerCase()) && (!E || E <= 7));
            if (I) {
                this.iframe = g('<iframe src="javascript:0" tabindex="-1" />').hide().appendTo("body")[0].contentWindow;
                this.navigate(F)
            }
            if (this._hasPushState) {
                g(window).bind("popstate", this.checkUrl)
            } else {
                if ("onhashchange" in window && !I) {
                    g(window).bind("hashchange", this.checkUrl)
                } else {
                    setInterval(this.checkUrl, this.interval)
                }
            }
            this.fragment = F;
            n = true;
            var J = window.location;
            var H = J.pathname == this.options.root;
            if (this._wantsPushState && !this._hasPushState && !H) {
                this.fragment = this.getFragment(null, true);
                window.location.replace(this.options.root + "#" + this.fragment)
            } else {
                if (this._wantsPushState && this._hasPushState && H && J.hash) {
                    this.fragment = J.hash.replace(p, "");
                    window.history.replaceState({}, document.title, J.protocol + "//" + J.host + this.options.root + this.fragment)
                }
            }
            return this.loadUrl()
        },route: function(E, F) {
            this.handlers.unshift({route: E,callback: F})
        },checkUrl: function(F) {
            var E = this.getFragment();
            if (E == this.fragment && this.iframe) {
                E = this.getFragment(this.iframe.location.hash)
            }
            if (E == this.fragment || E == decodeURIComponent(this.fragment)) {
                return false
            }
            if (this.iframe) {
                this.navigate(E)
            }
            this.loadUrl() || this.loadUrl(window.location.hash)
        },loadUrl: function(G) {
            var F = this.fragment = this.getFragment(G);
            var E = D.any(this.handlers, function(H) {
                if (H.route.test(F)) {
                    H.callback(F);
                    return true
                }
            });
            return E
        },navigate: function(E, F) {
            var H = (E || "").replace(p, "");
            if (this.fragment == H || this.fragment == decodeURIComponent(H)) {
                return
            }
            if (this._hasPushState) {
                var G = window.location;
                if (H.indexOf(this.options.root) != 0) {
                    H = this.options.root + H
                }
                this.fragment = H;
                window.history.pushState({}, document.title, G.protocol + "//" + G.host + H)
            } else {
                window.location.hash = this.fragment = H;
                if (this.iframe && (H != this.getFragment(this.iframe.location.hash))) {
                    this.iframe.document.open().close();
                    this.iframe.location.hash = H
                }
            }
            if (F) {
                this.loadUrl(E)
            }
        }});
    c.View = function(E) {
        this.cid = D.uniqueId("view");
        this._configure(E || {});
        this._ensureElement();
        this.delegateEvents();
        this.initialize.apply(this, arguments)
    };
    var o = function(E) {
        return g(E, this.el)
    };
    var a = /^(\S+)\s*(.*)$/;
    var z = ["model", "collection", "el", "id", "attributes", "className", "tagName"];
    D.extend(c.View.prototype, c.Events, {tagName: "div",$: o,initialize: function() {
        },render: function() {
            return this
        },remove: function() {
            g(this.el).remove();
            return this
        },make: function(F, E, H) {
            var G = document.createElement(F);
            if (E) {
                g(G).attr(E)
            }
            if (H) {
                g(G).html(H)
            }
            return G
        },delegateEvents: function(I) {
            if (!(I || (I = this.events))) {
                return
            }
            g(this.el).unbind(".delegateEvents" + this.cid);
            for (var H in I) {
                var J = this[I[H]];
                if (!J) {
                    throw new Error('Event "' + I[H] + '" does not exist')
                }
                var G = H.match(a);
                var F = G[1], E = G[2];
                J = D.bind(J, this);
                F += ".delegateEvents" + this.cid;
                if (E === "") {
                    g(this.el).bind(F, J)
                } else {
                    g(this.el).delegate(E, F, J)
                }
            }
        },_configure: function(G) {
            if (this.options) {
                G = D.extend({}, this.options, G)
            }
            for (var H = 0, F = z.length; H < F; H++) {
                var E = z[H];
                if (G[E]) {
                    this[E] = G[E]
                }
            }
            this.options = G
        },_ensureElement: function() {
            if (!this.el) {
                var E = this.attributes || {};
                if (this.id) {
                    E.id = this.id
                }
                if (this.className) {
                    E["class"] = this.className
                }
                this.el = this.make(this.tagName, E)
            } else {
                if (D.isString(this.el)) {
                    this.el = g(this.el).get(0)
                }
            }
        }});
    var A = function(E, F) {
        var G = l(this, E, F);
        G.extend = this.extend;
        return G
    };
    c.Model.extend = c.Collection.extend = c.Router.extend = c.View.extend = A;
    var v = {create: "POST",update: "PUT","delete": "DELETE",read: "GET"};
    c.sync = function(I, F, E) {
        var G = v[I];
        var H = D.extend({beforeSend: CopperEgg.httpAuth,type: G,dataType: "json",processData: false}, E);
        if (!H.url) {
            H.url = u(F) || w()
        }
        if (!H.data && F && (I == "create" || I == "update")) {
            H.contentType = "application/json";
            H.data = JSON.stringify(F.toJSON())
        }
        if (c.emulateJSON) {
            H.contentType = "application/x-www-form-urlencoded";
            H.processData = true;
            H.data = H.data ? {model: H.data} : {}
        }
        if (c.emulateHTTP) {
            if (G === "PUT" || G === "DELETE") {
                if (c.emulateJSON) {
                    H.data._method = G
                }
                H.type = "POST";
                H.beforeSend = function(J) {
                    J.setRequestHeader("X-HTTP-Method-Override", G)
                }
            }
        }
        return g.ajax(H)
    };
    var h = function() {
    };
    var l = function(F, E, G) {
        var H;
        if (E && E.hasOwnProperty("constructor")) {
            H = E.constructor
        } else {
            H = function() {
                return F.apply(this, arguments)
            }
        }
        D.extend(H, F);
        h.prototype = F.prototype;
        H.prototype = new h();
        if (E) {
            D.extend(H.prototype, E)
        }
        if (G) {
            D.extend(H, G)
        }
        H.prototype.constructor = H;
        H.__super__ = F.prototype;
        return H
    };
    var u = function(E) {
        if (!(E && E.url)) {
            return null
        }
        return D.isFunction(E.url) ? E.url() : E.url
    };
    var w = function() {
        throw new Error('A "url" property or function must be specified')
    };
    var d = function(G, F, E) {
        return function(H) {
            if (G) {
                G(F, H, E)
            } else {
                F.trigger("error", F, H, E)
            }
        }
    };
    var q = function(E) {
        return E.replace(/&(?!\w+;|#\d+;|#x[\da-f]+;)/gi, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#x27").replace(/\//g, "&#x2F;")
    }
}).call(this);