import React, { useEffect, useState } from "react";
import { useSelector, useDispatch } from "react-redux";

import { resetAccount } from "../../slices/account";

import { Drawer, IconButton, Grid, Button } from "@mui/material";
import { Menu, ChevronLeft } from "@mui/icons-material";

import AccountTable from "./AccountTable";
import AccountModal from "./Modal/AccountModal";

import { deleteAccount } from "../../api/Accounts/Account";
import { login, logout, getStatus } from "../../api/Accounts/Driver";

const SideBar = () => {
	const [open, setOpen] = useState(false);
	const [status, setStatus] = useState(false);

	const dispatch = useDispatch();
	const account = useSelector((state) => state.account.info);

	useEffect(() => {
		if (account.id !== -1) {
			getStatus(account.id).then((status) => setStatus(status));
		}
	}, [account.id]);

	const handleDrawerOpen = () => {
		setOpen(true);
	};

	const handleDrawerClose = () => {
		setOpen(false);
	};

	const onDelete = async () => {
		dispatch(resetAccount);
		await deleteAccount(account.id);
	};

	const onLog = async () => {
		if (status === true) {
			await logout(account.id);
			setStatus(await getStatus(account.id));
		} else {
			await login(account.id);
			setStatus(await getStatus(account.id));
		}
	};

	return (
		<>
			<IconButton
				color="inherit"
				aria-label="open drawer"
				onClick={handleDrawerOpen}
				edge="start"
				sx={{ mr: 2, ...(open && { display: "none" }) }}
			>
				<Menu />
			</IconButton>
			<Drawer anchor="left" open={open}>
				<IconButton onClick={handleDrawerClose}>
					<ChevronLeft />
				</IconButton>
				<AccountTable />
				<Grid container style={{ textAlign: "center" }}>
					<Grid item xs={12}>
						<AccountModal editMode={false} />
					</Grid>
					<Grid item xs={6}>
						<AccountModal editMode={true} />
					</Grid>
					<Grid item xs={6}>
						<Button onClick={onDelete}>Delete</Button>
					</Grid>
					<Grid item xs={12}>
						<Button onClick={onLog}>
							{status === true ? "Logout" : "Login"}
						</Button>
					</Grid>
				</Grid>
			</Drawer>
		</>
	);
};

export default SideBar;
