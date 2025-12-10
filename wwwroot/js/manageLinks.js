//SID: 2408078
const availableList = document.getElementById('available-list');
const assignedList = document.getElementById('assigned-list');
const assignButton = document.getElementById('btn-assign');
const unassignButton = document.getElementById('btn-unassign');
function moveSelected(fromBox, toBox) {
    const selectedOptions = Array.from(fromBox.selectedOptions);

    for (const option of selectedOptions) {
        fromBox.remove(option.index);
        toBox.add(option);
    }
}

assignButton.addEventListener('click', () => {
    moveSelected(availableList, assignedList);
});

unassignButton.addEventListener('click', () => {
    moveSelected(assignedList, availableList);
});

function selectAllAssigned() {
    const assignedOptions = assignedList.options;
    for (let i = 0; i < assignedOptions.length; i++) {
        assignedOptions[i].selected = true;
    }
}