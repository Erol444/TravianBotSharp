import React, { useEffect, useState } from "react";
import {
	Table,
	TableBody,
	TableCell,
	TableContainer,
	TableHead,
	TableRow,
	Box,
} from "@mui/material";
// import VillageRows from "./VillageRows";
import { useDispatch /* , useSelector */ } from "react-redux";

import style from "../../../styles/box";

const VillagesTable = () => {
	// const account = useSelector((state) => state.account.info);
	// const [villages, setVillages] = useState([]);
	const [selected /*, setSelected */] = useState(-1);
	const dispatch = useDispatch();

	/* const onClick = (vill) => {
        setSelected(vill.id);
    }; */

	useEffect(() => {
		// getAccounts().then((data) => setAccounts(data));
		if (selected !== -1) {
			// dispatch(fetchAccountByID(selected));
		}
	}, [selected, dispatch]);

	return (
		<>
			<Box sx={style}>
				<TableContainer>
					<Table size="small">
						<TableHead>
							<TableRow>
								<TableCell>Name</TableCell>
								<TableCell>Coords</TableCell>
							</TableRow>
						</TableHead>
						<TableBody>
							{/* <AccountRow
                                villages={villages}
                                handler={onClick}
                                selected={selected}
                            /> */}
						</TableBody>
					</Table>
				</TableContainer>
			</Box>
		</>
	);
};

export default VillagesTable;
