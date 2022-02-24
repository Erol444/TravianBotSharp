import React from "react";
import ReactDOM from "react-dom";
import { BrowserRouter } from "react-router-dom";
import App from "./App";
import * as serviceWorkerRegistration from "./serviceWorkerRegistration";

import { Provider } from "react-redux";
import store from "./store";
import axios from "axios";

axios.defaults.baseURL = "api/";

const baseUrl = document.getElementsByTagName("base")[0].getAttribute("href");
const rootElement = document.getElementById("root");

ReactDOM.render(
	<Provider store={store}>
		<BrowserRouter basename={baseUrl}>
			<App />
		</BrowserRouter>
	</Provider>,
	rootElement
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://cra.link/PWA
serviceWorkerRegistration.unregister();

if (module.hot) {
	module.hot.accept(); // already had this init code

	module.hot.addStatusHandler((status) => {
		if (status === "prepare") {
			console.log("=============================");
			console.log("Reloaded");
		}
	});
}
