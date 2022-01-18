import {
	Table,
	TableBody,
	TableCell,
	TableContainer,
	TableHead,
	TableRow,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import AccountRow from "./AccountRow";
import { getAccounts } from "../../api/Accounts/Account";

import { useDispatch, useSelector } from "react-redux";
import { fetchAccountByID } from "../../slices/account";

const AccountTable = () => {
	const account = useSelector((state) => state.account.info);
	const [accounts, setAccounts] = useState([]);
	const [selected, setSelected] = useState(account.id);
	const dispatch = useDispatch();

	const onClick = (acc) => {
		setSelected(acc.id);
	};

	useEffect(() => {
		getAccounts().then((data) => setAccounts(data));
		if (selected !== -1) {
			dispatch(fetchAccountByID(selected));
		}
	}, [selected, dispatch]);

	return (
		<>
			<TableContainer>
				<Table size="small">
					<TableHead>
						<TableRow>
							<TableCell>Username</TableCell>
							<TableCell>Server url</TableCell>
						</TableRow>
					</TableHead>
					<TableBody>
						<AccountRow
							accounts={accounts}
							handler={onClick}
							selected={selected}
						/>
					</TableBody>
				</Table>
			</TableContainer>
		</>
	);
};

export default AccountTable;
