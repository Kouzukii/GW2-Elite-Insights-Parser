/*jshint esversion: 6 */
var Layout = function (desc) {
    this.desc = desc;
    this.tabs = null;
};

Layout.prototype.addTab = function (tab) {
    if (this.tabs === null) {
        this.tabs = [];
    }
    this.tabs.push(tab);
};

var Tab = function (name, options) {
    this.name = name;
    options = options ? options : {};
    this.layout = null;
    this.desc = options.desc ? options.desc : null;
    this.active = options.active ? options.active : false;
    this.dataType =
        typeof options.dataType !== "undefined" ? options.dataType : -1;
};

var compileLayout = function () {
    // Compile
    Vue.component("general-layout-component", {
        name: "general-layout-component",
        template: "#tmplGeneralLayout",
        props: ["layout", "phase"],
        methods: {
            select: function (tab, tabs) {
                for (var i = 0; i < tabs.length; i++) {
                    tabs[i].active = false;
                }
                tab.active = true;
            }
        },
        computed: {
            layoutName: function () {
                if (!this.phase) {
                    return this.layout.desc;
                }
                return this.layout.desc ?
                    this.phase.name + " " + this.layout.desc :
                    this.phase.name;
            }
        }
    });
    //
    var layout = new Layout("Summary");
    // general stats
    var stats = new Tab("General Stats", {
        active: true
    });
    var statsLayout = new Layout(null);
    statsLayout.addTab(
        new Tab("Damage Stats", {
            active: true,
            dataType: DataTypes.damageTable
        })
    );
    statsLayout.addTab(
        new Tab("Gameplay Stats", {
            dataType: DataTypes.gameplayTable
        })
    );
    statsLayout.addTab(
        new Tab("Damage Modifiers Stats", {
            dataType: DataTypes.dmgModifiersTable
        })
    );
    statsLayout.addTab(
        new Tab("Defensive Stats", {
            dataType: DataTypes.defTable
        })
    );
    statsLayout.addTab(
        new Tab("Support Stats", {
            dataType: DataTypes.supTable
        })
    );
    stats.layout = statsLayout;
    layout.addTab(stats);
    // buffs
    var buffs = new Tab("Buffs");
    var buffLayout = new Layout(null);
    buffLayout.addTab(
        new Tab("Boons", {
            active: true,
            dataType: DataTypes.boonTable
        })
    );
    buffLayout.addTab(new Tab("Offensive Buffs", {
        dataType: DataTypes.offensiveBuffTable
    }));
    buffLayout.addTab(new Tab("Defensive Buffs", {
        dataType: DataTypes.defensiveBuffTable
    }));
    buffLayout.addTab(new Tab("Personal Buffs", {
        dataType: DataTypes.personalBuffTable
    }));
    buffs.layout = buffLayout;
    layout.addTab(buffs);
    // mechanics
    var mechanics = new Tab("Mechanics", {
        dataType: DataTypes.mechanicTable
    });
    layout.addTab(mechanics);
    // graphs
    var graphs = new Tab("Graph", {
        dataType: DataTypes.dpsGraph
    });
    layout.addTab(graphs);
    // targets
    var targets = new Tab("Targets Summary", {
        dataType: DataTypes.targetTab
    });
    layout.addTab(targets);
    // player
    var player = new Tab("Player Summary", {
        dataType: DataTypes.playerTab
    });
    layout.addTab(player);
    return layout;
};