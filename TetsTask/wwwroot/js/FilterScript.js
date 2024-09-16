document.addEventListener('DOMContentLoaded', function () {
    const filterInput = document.getElementById('filterInput');
    const applyFilterButton = document.getElementById('applyFilter');
    const table = document.getElementById('personsTable');
    const tbody = table.querySelector('tbody');

    applyFilterButton.addEventListener('click', function () {
        const filterValue = filterInput.value.toLowerCase();
        const rows = tbody.getElementsByTagName('tr');

        Array.from(rows).forEach(row => {
            const cells = row.getElementsByTagName('td');
            let rowContainsFilterText = false;

            Array.from(cells).forEach(cell => {
                if (cell.textContent.toLowerCase().includes(filterValue)) {
                    rowContainsFilterText = true;
                }
            });

            row.style.display = rowContainsFilterText ? '' : 'none';
        });
    });
});
