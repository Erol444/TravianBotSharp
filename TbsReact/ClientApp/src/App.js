import React, { useState, useEffect, useRef } from "react";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

import Layout from "./components/Layout";
import TokenInput from "./components/TokenInput";
import { changeAccount } from "./realtime/account";
import { usePrevious } from "./hooks/usePrevious";
import { useSelector } from "react-redux";

import { HubConnectionState } from "@microsoft/signalr/dist/esm/HubConnection";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { SignalRContext } from "./hooks/useSignalR";

const App = () => {
	toast.configure();
	const account = useSelector((state) => state.account.info.id);
	const prev = usePrevious(account);
	const toastId = useRef(null);

	const [connection, setConnection] = useState(null);

	useEffect(() => {
		const connect = new HubConnectionBuilder()
			.withUrl("/live")
			.withAutomaticReconnect()
			.build();
		setConnection(connect);
	}, [setConnection]);

	useEffect(() => {
		if (
			connection &&
			connection.state === HubConnectionState.Disconnected
		) {
			connection
				.start()
				.then(() => {
					connection.on("message", (message) => {
						console.log(message);
					});
				})
				.catch((error) => console.log(error));
		}
	}, [connection]);

	useEffect(() => {
		if (connection && connection.state === HubConnectionState.Connected) {
			changeAccount(connection, account, prev);
		}
	}, [connection, account, prev]);

	const [token, setToken] = useState(false);

	useEffect(() => {
		if (account === -1 && token) {
			toastId.current = toast.warning(
				"Selecte account by clicking on left icon",
				{ autoClose: false }
			);
		} else {
			if (toastId.current !== null) {
				toast.dismiss(toastId.current);
				toastId.current = null;
			}
		}
	}, [account, token]);

	return (
		<>
			<SignalRContext.Provider value={connection}>
				{token === false ? (
					<TokenInput setToken={setToken} />
				) : (
					<Layout />
				)}
			</SignalRContext.Provider>
		</>
	);
};

export default App;
