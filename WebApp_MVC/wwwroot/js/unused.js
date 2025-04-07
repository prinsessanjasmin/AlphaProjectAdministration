//<script src="https://kit.fontawesome.com/584828937d.js" crossorigin="anonymous"></script>

//<script>
//  var quill = new Quill('#editor', {
//    theme: 'snow' // 'bubble' is another option
//  });
//</script>
////Suggestion from ChatGPT 4o

//<script>
//document.addEventListener("DOMContentLoaded", function () {
//    let daySelect = document.getElementById("day");
//    let monthSelect = document.getElementById("month");
//    let yearSelect = document.getElementById("year");

//    // Populate Day (1-31)
//    for (let i = 1; i <= 31; i++) {
//        let option = new Option(i, i);
//        daySelect.add(option);
//    }

//    // Populate Month (January - December)
//    let monthNames = [
//        "January", "February", "March", "April", "May", "June",
//        "July", "August", "September", "October", "November", "December"
//    ];
//    monthNames.forEach((month, index) => {
//        let option = new Option(month, index + 1);
//        monthSelect.add(option);
//    });

//    // Populate Year (From 1900 to Current Year)
//    let currentYear = new Date().getFullYear();
//    for (let i = currentYear; i >= 1900; i--) {
//        let option = new Option(i, i);
//        yearSelect.add(option);
//    }
//});
//</script>