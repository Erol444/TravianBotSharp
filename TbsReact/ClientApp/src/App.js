import React, { useState, useEffect, useRef } from "react";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

import Layout from "./components/Layout";
import TokenInput from "./components/TokenInput";
import { signalRConnection, initConnection } from "./realtime/connection";
import { changeAccount } from "./realtime/account";
import { usePrevious } from "./hooks/usePrevious";
import { useDispatch, useSelector } from "react-redux";

import { HubConnectionState } from "@microsoft/signalr/dist/esm/HubConnection";

const App = () => {
	toast.configure();
	const account = useSelector((state) => state.account.info.id);
	const prev = usePrevious(account);
	const dispatch = useDispatch();
	const toastId = useRef(null);
	useEffect(() => {
		initConnection();
		signalRConnection
			.start()
			.then(() => {
				signalRConnection.on("message", (data) => console.log(data));
			})
			.catch((err) => {
				console.error(err);
			});
	}, [dispatch]);

	useEffect(() => {
		if (signalRConnection.State === HubConnectionState.Connected) {
			changeAccount(account, prev);
		}
	}, [account, prev]);

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
		<>{token === false ? <TokenInput setToken={setToken} /> : <Layout />}</>
	);
};

export default App;
