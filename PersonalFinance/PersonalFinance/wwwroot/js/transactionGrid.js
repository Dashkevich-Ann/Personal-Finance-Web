(function ($) {
    function Index() {
        $(document).ready(initGrid);

        function initGrid() {
            const transactionUrl = '/api/transaction/'
            $.get(transactionUrl).done(setUpGrid);


            function setUpGrid(data) {

                var filteredData = [...data];

                const dataTable = $('#transactionTable').DataTable({
                        data: filteredData,
                        paging: false,
                        scrollY: 400,
                        info: false,
                        searching: true,
                        columns: [
                            { data: 'date' },
                            { data: 'category.type' },
                            { data: 'category.name' },
                            { data: 'comment'},
                            { data: 'displayAmount' },
                            { data: 'transactionId' },
                            { data: 'amount'},
                        ],
                        columnDefs: [{
                            targets: 0,
                            render: $.fn.dataTable.render.moment('MMMM Do YYYY')
                        },
                            {
                                targets: 1,
                                render: function (data) {
                                    return data == 0 ? 'Income' : 'Cost';
                                }
                            },
                            {
                                targets: [5, 6],
                                visible: false,
                                searchable: false,
                            }
                        ],
                        buttons: [{
                            extend: 'pdfHtml5',
                            title: 'My transactions',
                            filename: 'My transactions',
                            text: "Export to PDF",
                            pageSize: 'LEGAL',
                            exportOptions: {
                                columns: ':visible',
                                stripHtml: true,
                            },
                        }],
                        footerCallback: footer
                })

                dataTable.buttons().container().appendTo('#transactionBtnGroup');

                setDeleteAction(dataTable)

            }

            function setDeleteAction(dataTable) {
                $('#transactionTable tbody').on('click', 'tr', function () {
                    if ($(this).hasClass('selected')) {
                        $(this).removeClass('selected');
                    }
                    else {
                        dataTable.$('tr.selected').removeClass('selected');
                        $(this).addClass('selected');
                    }
                });

                $('#delete-button').click(function () {
                    var transactionRow = dataTable.row('.selected');
                    if (transactionRow) {
                        debugger;
                        var data = transactionRow.data();

                    }
                });
            }

            function footer (row, data, start, end, display) {
                var api = this.api();

                // Remove the formatting to get integer data for summation
                var intVal = function (i) {
                    return typeof i === 'string' ?
                        i.replace(/[\$,]/g, '') * 1 :
                        typeof i === 'number' ?
                            i : 0;
                };

                // Total over this page
                total = api
                    .column(4, { search: 'applied' })
                    .data()
                    .reduce(function (a, b) {
                        return intVal(a) + intVal(b);
                    }, 0);

                countEntires = api
                    .column(0)
                    .data().length;

                countShown= api
                    .column(0, { search: 'applied' })
                    .data().length;

                // Update footer
                $(api.column(4).footer()).html(
                    '$' + total + ' total'
                );

                $(api.column(0).footer()).html(
                    'Shown ' + countShown + ' from ' + countEntires
                );
            }
        }
    }

    $(function () {
        var self = new Index();
        self.init();
    });
}(jQuery));