
/*

TODO: Replace functions with options object
TODO: Text selectable zones
TODO: Options to disable printing

*/

// Call this function to disable right clicking (to better simulate a true application)
const disableRightClick = function() {
	// Snippet from https://stackoverflow.com/questions/737022/how-do-i-disable-right-click-on-my-web-page
	document.addEventListener("contextmenu", event => event.preventDefault());
}

const disableDrag = function() {
	console.log("[DEBUG] Disabled drag");

	const classesToDisable = ["no-drag"];
	const tagsToDisable = ["img"];


	const elements = document.getElementsByTagName("img");

	for (let i = 0; i < elements.length; i++) {
		elements[i].setAttribute("draggable", false);
	}

	console.log(elements);
}