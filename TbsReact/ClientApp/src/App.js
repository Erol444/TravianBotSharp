import React, { useState, useEffect } from "react";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import axios from "axios";
import { setHeaderToken } from "./api/axios";

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

	const [token, setToken] = useState(axios.defaults.headers.common.token);

	useEffect(() => {
		setHeaderToken(token);
	}, [token]);

	return (
		<>
			{token === undefined ? (
				<TokenInput setToken={setToken} />
			) : (
				<Layout />
			)}
		</>
	);
};

export default App;
