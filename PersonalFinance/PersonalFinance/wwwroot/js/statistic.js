(function ($) {
    function index() {
        $(document).ready(init);

        function init() {
            loadData();

            function loadData() {
                const transactionUrl = '/api/transaction/categories/histories/6';
                $.get(transactionUrl).done(initChart);
            }

            function initChart(result) {
                var data = mapHistoryData(result);
                const config = {
                    type: 'line',
                    data: data,
                    options: {
                        responsive: true,
                        plugins: {
                            tooltip: {
                                mode: 'index',
                                intersect: false
                            },
                            legend: {
                                position: 'top',
                            },
                            title: {
                                display: false,
                                text: 'Cost Incomes'
                            }
                        }
                    },
                };

                const lineChart = new Chart($('#six-month-chart'), config);
            }

            function mapHistoryData(data) {
                var incomeDataItem = {
                    label: "Incomes",
                    data: getChartData(c => c.type === 0),
                    borderColor: '#0000FF',
                    backgroundColor: '#0000FF',
                }
                var costsDataItem = {
                    label: "Costs",
                    data: getChartData(c => c.type === 1),
                    borderColor: '#FF0000',
                    backgroundColor: '#FF0000',
                }

                var labels = data[0].categoryHistory.sort(sortDates).map(c => formatDate(c.monthYear));

                function getChartData(filterFn) {
                    var reduced = data.filter(filterFn).reduce((a, b) => {
                                return a.concat(b.categoryHistory);
                            },
                            [])
                        .sort(sortDates)
                        .reduce((acc, b) => {
                            var field = formatDate(b.monthYear);

                            if (acc.hasOwnProperty(field)) {
                                acc[field] += (b.amount * 1);
                            } else {
                                acc[field] = (b.amount * 1);
                            }
                            return acc;
                        },
                        {});

                    return Object.entries(reduced).sort((a, b) => sortDates(a[0], b[0]))
                        .map(c => c[1]);
                }


                return {
                    labels: labels,
                    datasets: [incomeDataItem, costsDataItem]
                };
            }

        }

        function sortDates(a, b) {
            return new Date(a.monthYear) - new Date(b.monthYear);
        };

        function formatDate(d) {
           return moment(d).format('MMM YYYY');
        }
    }
    $(index);
}(jQuery));

(function ($) {
    function index() {
        const ChartColors = [
            '#800000',
            '#8B008B',
            '#BA55D3',
            '#FF00FF',
            '#DA70D6',
            '#C71585',
            '#DB7093',
            '#FF1493',
            '#CD853F',
            '#DC143C',
            '#FF0000',
            '#F08080',
            '#FF4500',
            '#FFFF00',
            '#9ACD32',
            '#FF8C00',
            '#FFD700',
            '#B8860B',
            '#BDB76B',
            '#9ACD32',
            '#00FA9A',
            '#2E8B57',
            '#20B2AA',
            '#008080',
            '#7CFC00',
            '#008000',
            '#228B22',
            '#00FF00',
            '#98FB98',
            '#8FBC8F',
            '#DEB887',
            '#708090',
            '#191970',
            '#0000FF',
            '#8A2BE2',
            '#4B0082',
            '#9370DB',
            '#F4A460',
            '#808080'
        ];
        const Charts = [{ id: '#incomes-chart', type: 0, label: 'Incomes' }, { id: '#costs-chart', type: 1, label: 'Costs' }];

        $(document).ready(init);

        function init() {
            var date = $('#month-select').val();
            loadData(date, initCharts);
        }

        function loadData(date, callback) {
            const transactionUrl = '/api/transaction/categories/histories/month/' + date;
            return $.get(transactionUrl).done(callback);
        }

        function redrawCharts(result) {
            Charts.forEach(c => {
                if (c.chart) {
                    var data = mapHistoryData(result, c.type);

                    c.chart.data = data;
                    c.chart.update();
                }
            })
        }

        function initCharts(result) {
            Charts.forEach(c => initChart(result, c));
        }

        function initChart(result, opt) {
            var data = mapHistoryData(result, opt.type);

            const config = {
                type: 'pie',
                data: data,
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: 'top',
                        },
                    }
                },
            };

            opt.chart = new Chart($(opt.id), config);
        }

        function mapHistoryData(data, type) {

            const transactionType = {
                0: "Incomes",
                1: "Costs"
            }

            var dataValues = getChartData(c => c.type === type);
            var colors = ChartColors.slice(0, dataValues.length);

            var dataItems = {
                label: transactionType[type],
                data: dataValues.map(d => d.value),
                backgroundColor: colors,
                borderColor: colors
            }

            var labels = dataValues.map(c => c.label);

            function getChartData(filterFn) {
                return data.filter(filterFn).reduce((acc, b) => {
                            var item = {
                                label: b.name,
                                value: b.categoryHistory
                                    .reduce((p, v) => {
                                        return p + v.amount * 1;
                                    }, 0)
                            };
                    acc.push(item);
                    return acc;
                }, []).sort((a, b) => a.label.localeCompare(b.label));
            }

            return {
                labels: labels,
                datasets: [dataItems]
            };
        }


        $('#month-select').on('change', function (e) {
            loadData($(e.target).val(), redrawCharts);
        });

        var specialElementHandlers = {
            '.ignore-pdf': function (element, renderer) {
                return true;
            }
        };

        $('#save-pdf-bth').on('click',
            function(e) {
                e.preventDefault();
                var gridsContainer = $('#statistic');
                var date = $('#month-select').val();

                gridsContainer.find('.ignore-pdf').toggle(false);
                window.scrollTo(0, 0);
                html2canvas(gridsContainer,
                    {
                        onrendered: function (canvas) {
                            var width = canvas.width;
                            var height = canvas.height;
                            var millimeters = {};
                            millimeters.width = Math.floor(width * 0.264583);
                            millimeters.height = Math.floor(height * 0.264583);

                            var imgData = canvas.toDataURL(
                                'image/png');
                            var doc = new jsPDF('p', 'mm');
                            doc.deletePage(1);
                            doc.addPage(millimeters.width + 30, millimeters.height + 30);
                            doc.addImage(imgData, 'PNG', 15, 15);
                            doc.save(date + ' statistic.pdf');
                            gridsContainer.find('.ignore-pdf').toggle(true);
                        }

                    });
            });
    }
    $(index);
}(jQuery));

