function populateDropdowns() {
    let daySelect = document.getElementById("day");
    let monthSelect = document.getElementById("month");
    let yearSelect = document.getElementById("year");

    while (daySelect.options.length > 1) {
        daySelect.remove(1);
    }
    while (monthSelect.options.length > 1) {
        monthSelect.remove(1);
    }
    while (yearSelect.options.length > 1) {
        yearSelect.remove(1);
    }

    // Populate Day (1-31)
    for (let i = 1; i <= 31; i++) {
        let option = new Option(i, i);
        daySelect.add(option);
    }

    // Populate Month (January - December)
    let monthNames = [
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];
    monthNames.forEach((month, index) => {
        let option = new Option(month, index + 1);
        monthSelect.add(option);
    });

    // Populate Year (From 1900 to Current Year)
    let currentYear = new Date().getFullYear();
    for (let i = currentYear; i >= 1900; i--) {
        let option = new Option(i, i);
        yearSelect.add(option);
    }
}

