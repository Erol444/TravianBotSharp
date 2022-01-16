import React, { useState, useEffect } from "react";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

import Layout from "./components/Layout";

import { signalRConnection, initConnection } from "./realtime/connection";
import { changeAccount } from "./realtime/account";
import { usePrevious } from "./hooks/usePrevious";
const App = () => {
	toast.configure();

	const [selected, setSelected] = useState(-1);
	const [joined, setJoined] = useState(false);
	const prev = usePrevious(selected);

	// look complicated
	// may change later when i "pro" React ._.
	// - Vinaghost
	useEffect(() => {
		if (joined === false) {
			const join = async () => {
				try {
					initConnection();
					await signalRConnection.start();
					setJoined(true);
					signalRConnection.on("message", (data) => {
						console.log(data);
					});
				} catch (e) {
					console.log(e);
				}
			};

			join();
		}
	}, [joined]);

	useEffect(() => {
		if (joined === true) {
			changeAccount(selected, prev);
		}
		// eslint-disable-next-line react-hooks/exhaustive-deps
	}, [selected, joined]);

	return (
		<Layout
			selected={selected}
			setSelected={setSelected}
			isConnect={joined}
		></Layout>
	);
};

export default App;
