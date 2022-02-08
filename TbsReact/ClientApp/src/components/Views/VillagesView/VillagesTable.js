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

import { getVillages } from "../../../api/Village";

import VillageRows from "./VillageRows";

import { useDispatch, useSelector } from "react-redux";
import { fetchVillageByID } from "../../../slices/village";

import style from "../../../styles/box";

const VillagesTable = () => {
	const account = useSelector((state) => state.account.info);
	const village = useSelector((state) => state.village.info);
	const [villages, setVillages] = useState([]);
	const [selected, setSelected] = useState(village.id);
	const dispatch = useDispatch();

	const onClick = (vill) => {
		setSelected(vill.id);
	};

	useEffect(() => {
		if (account.id !== -1) {
			getVillages(account.id).then((data) => setVillages(data));
			if (selected !== -1) {
				dispatch(fetchVillageByID(selected));
			}
		}
	}, [selected, dispatch, account.id]);

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
							<VillageRows
								villages={villages}
								handler={onClick}
								selected={selected}
							/>
						</TableBody>
					</Table>
				</TableContainer>
			</Box>
		</>
	);
};

export default VillagesTable;
