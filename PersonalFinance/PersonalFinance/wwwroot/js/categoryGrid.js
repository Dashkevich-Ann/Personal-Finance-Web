(function ($) {
    function Index() {
        $(document).ready(initGrid);

        function initGrid() {
            loadData(addColumns);

            function setUpGrid(data, columns) {
                var gridColumns = [
                    { data: 'name' },
                    { data: 'limitDisplayValue' },
                    { data: 'type' },
                    { data: 'categoryId' },
                ];

                var monthColumns = columns.map(c => ({ data: c + '.amount' }));

                gridColumns = gridColumns.concat(monthColumns);

                const groupColumn = 2;

                const dataTable = $('#categoryTable').DataTable({
                    data: data,
                    paging: false,
                    scrollY: 400,
                    info: false,
                    searching: true,
                    orderFixed: [2, 'asc'],
                    order: [[groupColumn, 'asc']],
                    columns: gridColumns,
                    columnDefs: [
                        {
                            targets: [2, 3],
                            visible: false,
                            searchable: false,

                        },
                        {
                            targets: [4,5,6],
                            createdCell: function (td, cellData, rowData, row, col) {
                                if (rowData.monthLimit && rowData.monthLimit < cellData) {
                                    $(td).addClass('text-danger');
                                    $(td).addClass('font-weight-bold');
                                }
                            }
                        }
                    ],
                    buttons: [{
                        extend: 'pdfHtml5',
                        title: 'My categories',
                        filename: 'My categories',
                        text: "Export to PDF",
                        className: 'btn-secondary',
                        pageSize: 'LEGAL',
                        exportOptions: {
                            columns: ':visible',
                            stripHtml: true,
                        },
                        
                    },
                        {
                            extend: 'csv',
                            title: 'My categories',
                            filename: 'My categories',
                            text: "Export to CSV",
                            className: 'btn-secondary',
                            exportOptions: {
                                columns: ':visible',
                                stripHtml: true,
                            },
                        }],
                    drawCallback: function (settings) {
                        var api = this.api();
                        var rows = api.rows().nodes();
                        var last = null;

                        api.column(groupColumn).data().each(function (group, i) {
                            if (last !== group) {
                                $(rows).eq(i).before(
                                    '<tr class="group"><td colspan="5">' + (group == 0 ? 'Incomes' : 'Costs') + '</td></tr>'
                                );

                                last = group;
                            }
                        });
                    }
                })

                dataTable.buttons().container().removeClass('btn-group flex-wrap').appendTo('#categoryBtnGroup');

                $('#categoryTable tbody').on('click', 'tr', function () {
                    if ($(this).hasClass('group'))
                        return;

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
                const modalId = '#new-category-modal';

                $('#add-button').click(function (e) {
                        $(modalId).modal('show')
                        getCreateNewView();
                });

                function getCreateNewView() {
                    var url = '/finances/categories/create';

                    $.get(url).done(function (data) {
                        $(modalId).find('.modal-dialog').html(data);
                    });
                }

                $(document).on("click", "#submit-create-category", function (e) {
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
                            alert('Failed to create category');
                        }
                    })
                }
            }

            function setEditAction(dataTable) {
                const modalId = '#edit-category-modal';

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
                    var url = `/finances/categories/${data.categoryId}/type/${data.type}/edit`;

                    $.get(url).done(function (data) {
                        $(modalId).find(".modal-dialog").html(data);
                    });
                }

                $(document).on("click", "#submit-edit-category", function (e) {
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
                const modalId = '#remove-category-modal';

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

                $(document).on("click", "#categoryDeleteConfirm", function (e) {
                    submitRemove(e);
                })

                function getDeleteConfirm(data) {
                    var url = `/finances/categories/${data.categoryId}/type/${data.type}/delete`;

                    $.get(url).done(function (data) {
                        $(modalId).find(".modal-dialog").html(data);
                    });
                }

                function submitRemove(e) {
                    const elm = e.target;
                    var url = $(elm).data('url');
                    $.ajax({
                        type: 'DELETE',
                        url: url,
                        success: function (result) {
                            setTimeout(() => {
                                if (result == "Success") {
                                    $(modalId).modal('hide')
                                    dataTable.row('.selected').remove().draw();
                                }
                                else {
                                    alert('Failed to delete category');
                                }
                            }, 50)
                        },
                        error: function () {
                            alert('Failed to delete category');
                        }
                    })
                }
            }

            function loadData(callback) {
                const transactionUrl = '/api/transaction/categories/histories/3';
                $.get(transactionUrl).done(mapData);

                function mapData(data) {
                    var result = data.map(function(d) {
                        var r = Object.assign(d);

                        r.categoryHistory.forEach(function(c) {
                            r[moment(c.monthYear).format('MMM YYYY')] = c;
                        });

                        return r;

                    });

                    callback(result);
                }
            }

            function addColumns(result) {
                var columns = result[0].categoryHistory.map(function(c) {
                    return moment(c.monthYear).format('MMM YYYY');
                });

                var html = columns.reduce((p, v) => p + `<th scope="col">${v}</th>`, "");

                $('#categoryTable > thead > tr').append(html);

                setUpGrid(result, columns);
            }
        }
    }
    $(Index);
}(jQuery));