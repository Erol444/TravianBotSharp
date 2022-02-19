import { Grid, Button, Typography } from "@mui/material";
import React from "react";
import { useForm } from "react-hook-form";

import ContentBox from "../../../../ContentBox";
import MUISelect from "../../../../ref/MUISelect";

const PrerequisiteBuilding = () => {
	const { control, handleSubmit } = useForm();

	const onSubmit = (data) => {
		console.log(data);
	};

	const buildings = [
		{ id: 1, name: "Marketplace" },
		{ id: 2, name: "Embassy" },
	];
	return (
		<>
			<ContentBox>
				<Typography variant="h5">Prerequisite Building</Typography>
				<form onSubmit={handleSubmit(onSubmit)}>
					<Grid container spacing={3}>
						<Grid item xs={8}>
							<MUISelect
								label="Building"
								name="building"
								control={control}
								options={buildings}
								fullWidth
							/>
						</Grid>
						<Grid item xs={12}>
							<Button
								type="submit"
								variant="contained"
								value="submit"
								color="success"
							>
								Build
							</Button>
						</Grid>
					</Grid>
				</form>
			</ContentBox>
		</>
	);
};

export default PrerequisiteBuilding;
