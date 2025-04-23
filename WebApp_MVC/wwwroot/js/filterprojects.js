const startedProjects = document.findElementById('started-projects');
const completedProjects = document.findElementById('completed-projects');
const allProjects = document.findElementById('all-projects');

startedProjects.addEventListener('click', (event) => {
    startedProjects.classList.add('active');
});

completedProjects.addEventListener('click', (event) => {
    completedProjects.classList.add('active');
});