function UnityProgress(gameInstance, progress)
{

  if (!gameInstance.Module) {
	return;
  }

  if (!gameInstance.logo) {
    gameInstance.logo = document.createElement("div");
    gameInstance.logo.className = "logo " + gameInstance.Module.splashScreenStyle;
    gameInstance.container.appendChild(gameInstance.logo);
  }

  if (!gameInstance.progress) {
    gameInstance.progress = document.createElement("div");
    gameInstance.progress.className = "progress " + gameInstance.Module.splashScreenStyle;
    gameInstance.progress.empty = document.createElement("div");
    gameInstance.progress.empty.className = "empty";
    gameInstance.progress.appendChild(gameInstance.progress.empty);
    gameInstance.progress.full = document.createElement("div");
    gameInstance.progress.full.className = "full";
    gameInstance.progress.appendChild(gameInstance.progress.full);
    gameInstance.container.appendChild(gameInstance.progress);
  }

  gameInstance.progress.full.style.width = (100 * progress) + "%";
  gameInstance.progress.empty.style.width = (100 * (1 - progress)) + "%";

  if (progress == 1) {
    gameInstance.logo.style.display = gameInstance.progress.style.display = "none";
  }

}

function stringToBoolean(string)
{
	switch(string.toLowerCase().trim()){
		case "true": case "yes": case "1": return true;
		case "false": case "no": case "0": case null: return false;
		default: return false;
	}
}

function toggle_btn_fullscreen( show_btn )
{

	let btn_fullscreen = document.getElementById("btn_fullscreen");

	if( show_btn ) {
		btn_fullscreen.style.display = 'block';
	} else {
		btn_fullscreen.style.display = 'none';
	}


}
