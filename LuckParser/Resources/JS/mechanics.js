/*jshint esversion: 6 */

var compileMechanics = function () {
    Vue.component("mechanics-stats-component", {
        props: ["phase", "players", "enemies"],
        template: "#tmplMechanicsTable",
        data: function () {
            return {
                cacheP: new Map(),
                cacheE: new Map(),
                mechanicsData: mechanicMap,
            };
        },
        mounted() {
            initTable("#playermechs", 0, "asc");
            //
            if (this.enemyMechHeader.length) {
                initTable("#enemymechs", 0, "asc");
            }
        },
        updated() {
            updateTable("#playermechs");
            //
            if (this.enemyMechHeader.length) {
                updateTable("#enemymechs");
            }
        },
        computed: {
            playerMechHeader: function () {
                var playerMechanics = [];
                for (var i = 0; i < this.mechanicsData.length; i++) {
                    if (this.mechanicsData[i].playerMech) {
                        playerMechanics.push(this.mechanicsData[i]);
                    }
                }
                return playerMechanics;
            },
            playerMechRows: function () {
                if (this.cacheP.has(this.phase)) {
                    return this.cacheP.get(this.phase);
                }
                var phase = this.phase;
                var players = this.players;
                var rows = [];
                for (var i = 0; i < players.length; i++) {
                    var player = players[i];
                    if (player.isConjure) {
                        continue;
                    }
                    rows.push({
                        player: player,
                        mechs: phase.mechanicStats[i]
                    });
                }
                this.cacheP.set(this.phase, rows);
                return rows;
            },
            enemyMechHeader: function () {
                var enemyMechanics = [];
                for (var i = 0; i < this.mechanicsData.length; i++) {
                    if (this.mechanicsData[i].enemyMech) {
                        enemyMechanics.push(this.mechanicsData[i]);
                    }
                }
                return enemyMechanics;
            },
            enemyMechRows: function () {
                if (this.cacheE.has(this.phase)) {
                    return this.cacheE.get(this.phase);
                }
                var phase = this.phase;
                var enemies = this.enemies;
                var rows = [];
                for (var i = 0; i < enemies.length; i++) {
                    var enemy = enemies[i];
                    rows.push({
                        enemy: enemy.name,
                        mechs: phase.enemyMechanicStats[i]
                    });
                }
                this.cacheE.set(this.phase, rows);
                return rows;
            }
        }
    });
};