(function ($) {
    function Index() {
        $(document).ready(initGrid);

        function initGrid() {
            loadData(setUpGrid);

            function setUpGrid(data) {

                const dataTable = $('#transactionTable').DataTable({
                    data: data,
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
                    },
                        {
                            extend: 'csv',
                            title: 'My transactions',
                            filename: 'My transactions',
                            text: "Export to CSV",
                            className: 'btn-secondary',
                            exportOptions: {
                                columns: ':visible',
                                stripHtml: true,
                            },
                        }],
                    footerCallback: footer
                })

                dataTable.buttons().container().removeClass('btn-group flex-wrap').appendTo('#transactionBtnGroup');

                $('#transactionTable tbody').on('click', 'tr', function () {
                    if ($(this).hasClass('selected')) {
                        $(this).removeClass('selected');
                    }
                    else {
                        dataTable.$('tr.selected').removeClass('selected');
                        $(this).addClass('selected');
                    }
                });

                setDeleteAction(dataTable);
                setCreateAction(dataTable);
                setEditAction(dataTable);

            }

            function setCreateAction(dataTable) {
                const modalId = '#new-transaction-modal';

                $('#add-button').click(function (e) {
                        $(modalId).modal('show')
                        getCreateNewView();
                });

                function getCreateNewView() {
                    var url = '/finances/transaction/create';

                    $.get(url).done(function (data) {
                        $(modalId).find('.modal-dialog').html(data);
                    });
                }

                $(document).on("click", "#submitCreate", function (e) {
                    formSubmit(e);
                })

                function formSubmit(e) {
                    e.preventDefault();
                    var form = e.target.closest('form')
                    var data = $(form).serialize();
                    var url = $(form).attr('action');
                    console.log(data);
                    $.ajax({
                        type: 'POST',
                        url: url,
                        contentType: 'application/x-www-form-urlencoded; charset=UTF-8', // when we use .serialize() this generates the data in query string format. this needs the default contentType (default content type is: contentType: 'application/x-www-form-urlencoded; charset=UTF-8') so it is optional, you can remove it
                        data: data,
                        success: function (result) {
                            setTimeout(() => {
                                if (result === "Success") {
                                    loadData(function(d) {
                                        dataTable
                                            .clear()
                                            .rows.add(d)
                                            .draw();
                                    });
                                    $(modalId).modal('hide');
                                } else {
                                    $(modalId).find(".modal-dialog").html(result);
                                }
                            }, 50)
                        },
                        error: function () {
                            alert('Failed to update transaction');
                        }
                    })
                }
            }

            function setEditAction(dataTable) {
                const modalId = '#edit-transaction-modal';

                $('#edit-button').click(function(e) {
                    var transactionRow = dataTable.row('.selected');
                    var data = transactionRow.data();
                    if (data) {
                        $(modalId).modal('show');
                        getEditView(data);
                    }
                    else {
                        $(modalId).modal('hide');
                    }
                })

                function getEditView(data) {
                    var url = `/finances/transaction/${data.transactionId}/type/${data.category.type}/edit`;

                    $.get(url).done(function (data) {
                        $(modalId).find(".modal-dialog").html(data);
                    });
                }

                $(document).on("click", "#submitEdit", function (e) {
                    formSubmit(e);
                })

                function formSubmit(e) {
                    e.preventDefault();
                    var form = e.target.closest('form')
                    var data = $(form).serialize();
                    var url = $(form).attr('action');
                    console.log(data);
                    $.ajax({
                        type: 'POST',
                        url: url,
                        contentType:
                            'application/x-www-form-urlencoded; charset=UTF-8', // when we use .serialize() this generates the data in query string format. this needs the default contentType (default content type is: contentType: 'application/x-www-form-urlencoded; charset=UTF-8') so it is optional, you can remove it
                        data: data,
                        success: function(result) {
                            setTimeout(() => {
                                    if (result === "Success") {
                                        loadData(function(d) {
                                            dataTable
                                                .clear()
                                                .rows.add(d)
                                                .draw();
                                        });
                                        $(modalId).modal('hide');
                                    } else {
                                        $(modalId).find(".modal-dialog").html(result);
                                    }
                                },
                                50)
                        },
                        error: function() {
                            alert('Failed to update transaction');
                        }
                    })
                }

            }

            function setDeleteAction(dataTable) {
                const modalId = '#confirm-removal-modal';

                $('#delete-button').click(function (e) {
                    var transactionRow = dataTable.row('.selected');
                    var data = transactionRow.data();
                    if (data) {
                        $(modalId).modal('show')
                        getDeleteConfirm(data);
                    }
                    else {
                        $(modalId).modal('hide')
                    }
                });

                $(document).on("click", "#trDeleteConfirm", function (e) {
                    submitRemove(e);
                })

                function getDeleteConfirm(data) {
                    var url = `/finances/transaction/${data.transactionId}/type/${data.category.type}/delete`;

                    $.get(url).done(function (data) {
                        $(modalId).find(".modal-dialog").html(data);
                    });
                }

                function submitRemove(e) {
                    const elm = e.target;
                    var trId = $(elm).data('id');
                    var type = $(elm).data('type');
                    $.ajax({
                        type: 'DELETE',
                        url: `/api/transaction/${trId}/type/${type}`,
                        success: function (result) {
                            setTimeout(() => {
                                if (result.isSuccess) {
                                    $(modalId).modal('hide')
                                    dataTable.row('.selected').remove().draw();
                                }
                                else {
                                    alert('Failed to detele transaction');
                                }
                            }, 50)
                        },
                        error: function () {
                            alert('Failed to detele transaction');
                        }
                    })
                }
            }

            function loadData(callback) {
                const transactionUrl = '/api/transaction/';
                $.get(transactionUrl).done(callback);

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

    $(Index);
}(jQuery));