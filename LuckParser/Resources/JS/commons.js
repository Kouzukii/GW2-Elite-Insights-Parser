/*jshint esversion: 6 */

var roundingComponent = {
    methods: {
        round: function (value) {
            if (isNaN(value)) {
                return 0;
            }
            return Math.round(value);
        },
        round2: function (value) {
            if (isNaN(value)) {
                return 0;
            }
            var mul = 100;
            return Math.round(mul * value) / mul;
        },
        round3: function (value) {
            if (isNaN(value)) {
                return 0;
            }
            var mul = 1000;
            return Math.round(mul * value) / mul;
        }
    }
};

var compileCommons = function () {
    Vue.component('rotation-legend-component', {
        template: "#tmplRotationLegend"
    });

    Vue.component('dmgtaken-component', {
        props: ['actor', 'tableid',
            'phaseindex'
        ],
        template: "#tmplDamageTaken",
        computed: {
            dmgtaken: function () {
                return this.actor.details.dmgDistributionsTaken[this.phaseindex];
            }
        },
    });

    Vue.component("graph-component", {
        props: ['id', 'layout', 'data'],
        template: '<div :id="id"></div>',
        mounted: function () {
            var div = document.querySelector(this.queryID);
            Plotly.react(div, this.data, this.layout);
            var _this = this;
            div.on('plotly_animated', function () { Plotly.relayout(div, _this.layout); });
        },
        computed: {
            queryID: function () {
                return "#" + this.id;
            }
        },
        watch: {
            layout: {
                handler: function () {
                    var div = document.querySelector(this.queryID);
                    var duration = 1000;
                    Plotly.animate(div, { data: this.data }, {
                        transition: {
                            duration: duration,
                            easing: 'cubic-in-out'
                        },
                        frame: {
                            duration: 1.5 * duration
                        }
                    });
                },
                deep: true
            }
        }
    });
    Vue.component("buff-table-component", {
        props: ["buffs", "playerdata", "generation", "condition", "sums", "id"],
        template: "#tmplBuffTable",
        methods: {
            getAvgTooltip: function (avg) {
                if (avg) {
                    return (
                        "Average number of " +
                        (this.condition ? "conditions: " : "boons: ") +
                        avg
                    );
                }
                return false;
            },
            getCellTooltip: function (buff, val, uptime) {
                if (val instanceof Array) {
                    if (!uptime && this.generation && val[0] > 0) {
                        return val[1] + (buff.stacking ? "" : "%") + " with overstack";
                    } else if (buff.stacking && val[1] > 0) {
                        return "Uptime: " + val[1] + "%";
                    }
                }
                return false;
            },
            getCellValue: function (buff, val) {
                var value = val;
                if (val instanceof Array) {
                    value = val[0];
                }
                if (value > 0) {
                    return buff.stacking ? value : value + "%";
                }
                return "-";
            }
        },
        mounted() {
            initTable("#" + this.id, 0, "asc");
        },
        updated() {
            updateTable("#" + this.id);
        }
    });

    Vue.component("damagedist-table-component", {
        props: ["dmgdist", "tableid", "actor", "isminion", "istarget"],
        template: "#tmplDamageDistTable",
        data: function () {
            return {
                sortdata: {
                    order: "desc",
                    index: 2
                }
            };
        },
        mixins: [roundingComponent],
        mounted() {
            var _this = this;
            initTable(
                "#" + this.tableid,
                this.sortdata.index,
                this.sortdata.order,
                function () {
                    var order = $("#" + _this.tableid)
                        .DataTable()
                        .order();
                    _this.sortdata.order = order[0][1];
                    _this.sortdata.index = order[0][0];
                }
            );
        },
        beforeUpdate() {
            $("#" + this.tableid)
                .DataTable()
                .destroy();
        },
        updated() {
            var _this = this;
            initTable(
                "#" + this.tableid,
                this.sortdata.index,
                this.sortdata.order,
                function () {
                    var order = $("#" + _this.tableid)
                        .DataTable()
                        .order();
                    _this.sortdata.order = order[0][1];
                    _this.sortdata.index = order[0][0];
                }
            );
        },
        methods: {
            getSkill: function (isBoon, id) {
                return findSkill(isBoon, id);
            }
        }
    });
};
