import React from "react";
import { Grid } from "@mui/material";
import ContentBox from "../../../ContentBox";

const Build = () => {
	return (
		<ContentBox>
			<Grid container spacing={2}>
				<Grid item xs={4}>
					<ContentBox name="village building list" />
				</Grid>
				<Grid item xs={8}>
					<Grid container spacing={2}>
						<Grid item xs={3}>
							<ContentBox name="Button add building" />
						</Grid>
						<Grid item xs={9}>
							<ContentBox name="Current building" />
						</Grid>

						<Grid item xs={12}>
							<ContentBox name="Option" />
						</Grid>
						<Grid item xs={12}>
							<ContentBox name="Building queue" />
						</Grid>
					</Grid>
				</Grid>
			</Grid>
		</ContentBox>
	);
};

export default Build;
